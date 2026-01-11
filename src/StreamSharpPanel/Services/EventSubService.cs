using StreamSharpPanel.Static;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Nodes;
using StreamSharpPanel.Models;
using StreamSharpPanel.Models.NotificationEvents;
using static StreamSharpPanel.Static.General;

namespace StreamSharpPanel.Services;

public sealed class EventSubService(ILogger<EventSubService> logger, ApiCallerService api, SettingsService settings) : IAsyncDisposable
{
    private const int DefaultKeepAliveTimeoutSeconds = 10;
    private const int KeepAliveGracePeriodSeconds = 2;
    private const int ConnectionHealthcheckSeconds = 1;

    private readonly ILogger<EventSubService> _logger = logger;
    private readonly ApiCallerService _api = api;

    private Uri _currentConnectionUri = TwitchUris.EventSubUri;
    private ClientWebSocket _ws = new();
    private CancellationTokenSource? _cancSource;
    private TaskCompletionSource? _completionSource;

    private int _keepAliveTimeoutSeconds;
    private long _lastKeepAliveTimeStamp = DateTimeOffset.UtcNow.Ticks;

    private Task? _listenTask;
    private Task? _healtcheckTask;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly HashSet<string> _readMessages = [];
    private readonly Subject<TwitchNotification> _notificationStream = new();

    internal string CurrentSession { get; private set; } = "";
    internal string? UserId { get; private set; }
    public bool IsConnected { get; private set; }


    internal async Task StartListening()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (IsConnected)
                return;

            if (_cancSource is not null)
            {
                await _cancSource.CancelAsync();
                await Task.NullSafeInvoke(_listenTask);
            }
            
            _cancSource = new CancellationTokenSource();
            _completionSource = new TaskCompletionSource();

            UserId ??= settings.CurrentSettings.UserId ?? throw new Exception("User not found");

            _ws.Dispose();
            _ws = new();

            await _ws.ConnectAsync(_currentConnectionUri, _cancSource.Token);
            IsConnected = true;

            _listenTask = Listen(_cancSource.Token);
            await _completionSource.Task;
            await SubscribeToStandardEvents(UserId, _cancSource.Token);

            _healtcheckTask = Task.Run(() => ConnectionHealthCheck(_cancSource.Token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start listening to EventSub socket: ");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    internal async Task StopListening()
    {
        await Task.NullSafeInvoke(_cancSource?.CancelAsync());
        //await Task.NullSafeInvoke(_listenTask);
        IsConnected = false;
    }

    private async Task Listen(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_ws.State != WebSocketState.Open)
                {
                    _logger.LogCritical("An anomaly was detected during processing, please reconnect.");
                    _ws.Abort();
                    break;
                }

                await using var wsStream = WebSocketStream.CreateReadableMessageStream(_ws);
                var json = await JsonNode.ParseAsync(wsStream, cancellationToken: cancellationToken);
                var metadata = json?["metadata"]?.Deserialize<MessageMetadata>(JsonOptions);

                var invalidMessage = metadata is not null &&
                    _readMessages.Contains(metadata.MessageId) &&
                    metadata.MessageTimestamp.AddMinutes(10) < DateTime.Now;

                if (invalidMessage)
                    continue;

                var timestamp = metadata?.MessageTimestamp.Ticks ?? DateTimeOffset.UtcNow.Ticks;
                Interlocked.Exchange(ref _lastKeepAliveTimeStamp, timestamp);

                var payload = json!["payload"];
                IMessagePayload? msg = metadata!.MessageType switch
                {
                    "session_welcome" => payload.Deserialize<WelcomePayload>(JsonOptions),
                    "session_keepalive" => payload.Deserialize<KeepAlivePayload>(JsonOptions),
                    "session_reconnect" => payload.Deserialize<ReconnectPayload>(JsonOptions),
                    "notification" => payload.Deserialize<NotificationPayload>(JsonOptions),
                    "revocation" => payload.Deserialize<RevocationPayload>(JsonOptions),
                    _ => null
                };

                // todo: don't pass null case to handler
                if (await HandleMessage(msg, cancellationToken))
                {
                    _readMessages.Add(metadata.MessageId);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("The websocket listening task has been cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message: ");
            }
        }

        if (_ws.State < WebSocketState.CloseSent)
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
    }

    private async ValueTask<bool> HandleMessage(IMessagePayload? payload, CancellationToken cancellationToken)
    {
        switch (payload)
        {
            case WelcomePayload { Session: { Id: var id, KeepaliveTimeoutSeconds: var timeout } }:
                CurrentSession = id;
                _keepAliveTimeoutSeconds = timeout ?? DefaultKeepAliveTimeoutSeconds;

                Interlocked.Exchange(ref _lastKeepAliveTimeStamp, DateTimeOffset.UtcNow.UtcTicks);
                _completionSource?.TrySetResult();

                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Connected to session {Session}", CurrentSession);
                break;

            case KeepAlivePayload:
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Connection KeepAlive successful");
                break;

            case NotificationPayload notification:
                ProcessNotificationEvent(notification);
                break;

            case ReconnectPayload reconnect:
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Received reconnect event. Attempting reconnection now...");
        
                await SwitchConnection(reconnect, cancellationToken);
                break;

            case RevocationPayload revocation:
                _logger.LogError("The subscription {Sub} was removed citing the following reason: {Status}", revocation.Subscription.Id, revocation.Subscription.Status);
                _notificationStream.OnError(new RevocationException($"The subscription {revocation.Subscription.Id} was removed citing the following reason: {revocation.Subscription.Status}"));
                return false;

            default:
                throw new NotSupportedException($"Unknown payload type: {payload?.GetType()}");
        }

        return true;
    }

    // todo: how do we make sure no messages are dropped?    
    private async Task SwitchConnection(ReconnectPayload reconnect, CancellationToken cancellationToken)
    {
        var newWs = new ClientWebSocket();
        var newUri = new Uri(reconnect.Session.ReconnectUrl!);

        await newWs.ConnectAsync(newUri, cancellationToken);
        await using var wsStream = WebSocketStream.CreateReadableMessageStream(newWs);
        var json = await JsonNode.ParseAsync(wsStream, cancellationToken: cancellationToken);

        // todo: refactor with strong typing
        if (json?["metadata"]?["message_type"]?.GetValue<string>() == "session_welcome")
        {
            var welcome = json["payload"].Deserialize<WelcomePayload>(JsonOptions);

            // todo: make sure the deserialization succeeds
            CurrentSession = welcome.Session.Id;
            _currentConnectionUri = newUri;
            _keepAliveTimeoutSeconds = reconnect.Session.KeepaliveTimeoutSeconds ?? 10;

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Successfully reconnected to session {Session}", CurrentSession);
                _logger.LogInformation("Attempting to disconnect from expiring session...");
            }

            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
            _ws = newWs;

            _logger.LogInformation("Successfully disconnected from expiring session.");
        }
    }

    internal void MockNotification(TwitchNotification notification) => _notificationStream.OnNext(notification);

    private void ProcessNotificationEvent(NotificationPayload payload)
    {
        TwitchNotification? notification = payload.Subscription.Type switch
        {
            SubscriptionType.Channel.ChatMessage => payload.Event.Deserialize<ChannelChatMessage>(JsonOptions),
            SubscriptionType.Channel.Ban => payload.Event.Deserialize<ChannelBan>(JsonOptions),
            SubscriptionType.Channel.Unban => payload.Event.Deserialize<ChannelUnban>(JsonOptions),
            SubscriptionType.Channel.UnbanRequestCreate => payload.Event.Deserialize<ChannelUnbanRequestCreate>(JsonOptions),
            SubscriptionType.Channel.UnbanRequestResolve => payload.Event.Deserialize<ChannelUnbanRequestResolve>(JsonOptions),
            SubscriptionType.Channel.Cheer => payload.Event.Deserialize<ChannelCheer>(JsonOptions),
            SubscriptionType.Channel.Follow => payload.Event.Deserialize<ChannelChatFollow>(JsonOptions),
            SubscriptionType.Channel.Raid => payload.Event.Deserialize<ChannelRaid>(JsonOptions),
            SubscriptionType.Channel.Subscribe => payload.Event.Deserialize<ChannelSubscribe>(JsonOptions),
            SubscriptionType.Channel.SubscriptionGift => payload.Event.Deserialize<ChannelSubscriptionGift>(JsonOptions),
            SubscriptionType.Channel.SubscriptionMessage => payload.Event.Deserialize<ChannelSubscriptionMessage>(JsonOptions),
            SubscriptionType.Channel.ChannelPointsCustomRewardRedemption => payload.Event.Deserialize<ChannelPointsCustomRewardAdd>(JsonOptions),
            _ => new UnhandledNotification()
        };

        // todo: add an "OnError" handle?
        if (notification is not null)
        {
            if (notification is ChannelChatMessage msg)
                _logger.LogChatMessage(msg);

            _notificationStream.OnNext(notification);
        }
    }

    internal IDisposable OnEventReceived<T>(Action<T> subCallback, Func<T, bool>? subCondition = null) where T : TwitchNotification
    {
        return _notificationStream
            .OfType<T>()
            .Where(subCondition ?? (static _ => true))
            .Subscribe(subCallback);
    }

    // todo: add a session service to handle session data for both eventsub and api
    // todo: should this method be less tightly coupled with the current state?
    internal async Task SubscribeToStandardEvents(string broadcasterId, CancellationToken ct)
    {
        if (UserId is null)
            throw new InvalidOperationException("The user id has not been specified");

        await _api.Subscribe(SubscriptionType.Channel.ChatMessage, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);

        await _api.Subscribe(SubscriptionType.Channel.Ban, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        await _api.Subscribe(SubscriptionType.Channel.Unban, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        await _api.Subscribe(SubscriptionType.Channel.UnbanRequestCreate, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        await _api.Subscribe(SubscriptionType.Channel.UnbanRequestResolve, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        //await _api.Subscribe(SubscriptionType.Channel.ChatMessageDelete, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);

        await _api.Subscribe(SubscriptionType.Channel.Follow, "2", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        await _api.Subscribe(SubscriptionType.Channel.Cheer, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        await _api.Subscribe(SubscriptionType.Channel.Raid, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);

        await _api.Subscribe(SubscriptionType.Channel.Subscribe, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        await _api.Subscribe(SubscriptionType.Channel.SubscriptionGift, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
        await _api.Subscribe(SubscriptionType.Channel.SubscriptionMessage, "1", CurrentSession, UserId, broadcasterId, cancellationToken: ct);

        await _api.Subscribe(SubscriptionType.Channel.ChannelPointsAutomaticRewardRedemption, "2", CurrentSession, UserId, broadcasterId, cancellationToken: ct);
    }

    private async Task ConnectionHealthCheck(CancellationToken cancellationToken)
    {
        if (_keepAliveTimeoutSeconds <= 0)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("KeepAlive timeout is not set or invalid ({Timeout}), skipping health check.", _keepAliveTimeoutSeconds);

            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Starting ConnectionHealthCheck: keepaliveTimeout={Timeout}s, grace={Grace}s", _keepAliveTimeoutSeconds, KeepAliveGracePeriodSeconds);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(ConnectionHealthcheckSeconds));
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var lastTicks = Interlocked.Read(ref _lastKeepAliveTimeStamp);
            var lastTime = new DateTimeOffset(lastTicks, TimeSpan.Zero);
            var age = DateTimeOffset.UtcNow - lastTime;

            if (age.TotalSeconds > _keepAliveTimeoutSeconds + KeepAliveGracePeriodSeconds)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("KeepAlive not detected within {Timeout}s (+{Grace}s grace), aborting and reconnecting. lastKeepAlive={Last}", _keepAliveTimeoutSeconds, KeepAliveGracePeriodSeconds, lastTime);

                _currentConnectionUri = TwitchUris.EventSubUri;
                try
                {
                    _ws.Abort();
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "An exception occurred while aborting the websocket: ");
                }
                IsConnected = false;

                _ = StartListening();
                break;
            }
        }

        //var shouldReconnect = false;

        //using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_keepAliveTimeoutSeconds));
        //while (await timer.WaitForNextTickAsync(cancellationToken))
        //{
        //    if (Interlocked.Exchange(ref _keepAliveState, KeepAliveState.Dead) != KeepAliveState.Alive)
        //    {
        //        // Reconnection is needed
        //        _currentConnectionUri = TwitchUri.TwitchEventSubUri;

        //        _ws.Abort();
        //        IsConnected = false;

        //        shouldReconnect = true;
        //        break;
        //    }
        //}

        //if (shouldReconnect)
        //    _ = StartListening();
    }

    public async ValueTask DisposeAsync()
    {
        if (_ws.State < WebSocketState.Closed)
        {
            _ws.Abort();
        }
        else
        {
            await Task.NullSafeInvoke(_listenTask);
        }

        IsConnected = false;
        _ws.Dispose();
    }
}
