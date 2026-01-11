using Microsoft.AspNetCore.WebUtilities;
using StreamSharpPanel.Models;
using StreamSharpPanel.Models.ChatterInfo;
using StreamSharpPanel.Models.Configuration;
using StreamSharpPanel.Models.Http;
using StreamSharpPanel.Static;
using static StreamSharpPanel.Static.General;

namespace StreamSharpPanel.Services;

public class ApiCallerService(ILogger<ApiCallerService> logger, IHttpClientFactory http)
{
    internal async Task<string?> GetUserId(string username, CancellationToken cancellationToken)
    {
        using var client = http.CreateTwitchClient();
        var resp = await client.GetFromJsonAsync<GetUserResponse>($"users?login={username}", JsonOptions, cancellationToken);

        return resp?.Data.FirstOrDefault()?.Id;
    }

    internal async Task<StreamInfo> GetChannelInformation(string username, CancellationToken cancellationToken = default)
    {
        using var client = http.CreateTwitchClient();

        var settings = await client.GetFromJsonAsync<StreamSettingsDto>($"channels?broadcaster_id={username}", JsonOptions, cancellationToken);
        var streamInfoDto = settings?.Data.FirstOrDefault() ?? new();

        return new StreamInfo(streamInfoDto);
    }

    internal async Task<(bool Success, string? Message)> SetChannelInformation(string username, StreamInfo streamInfo, CancellationToken cancellationToken = default)
    {
        using var client = http.CreateTwitchClient();
        var resp = await client.PatchAsJsonAsync($"channels?broadcaster_id={username}", streamInfo, JsonOptions, cancellationToken);

        if (!resp.IsSuccessStatusCode)
        {
            var text = await resp.Content.ReadAsStringAsync(cancellationToken);
            return (false, text);
        }

        return (true, null);
    }

    internal async Task<TwitchCategory> GetCategory(string categoryId, CancellationToken cancellationToken = default)
    {
        using var client = http.CreateTwitchClient();
        var result = await client.GetFromJsonAsync<TwitchCategorySearchResult>($"games?id={categoryId}", JsonOptions, cancellationToken);

        return result?.Data.FirstOrDefault() ?? new();
    }

    internal async Task<TwitchCategorySearchResult> SearchCategory(string category, CancellationToken cancellationToken = default)
    {
        using var client = http.CreateTwitchClient();
        var query = QueryHelpers.AddQueryString("search/categories", "query", category);
        return await client.GetFromJsonAsync<TwitchCategorySearchResult>(query, JsonOptions, cancellationToken) ?? new();
    }

    internal async Task Subscribe(string type, string version, string session, string userId, string broadcasterId, string? moderatorId = null, CancellationToken cancellationToken = default)
    {
        moderatorId ??= broadcasterId;

        var body = new SubscriptionRequest(
            Type: type,
            Version: version,
            Condition: new Condition(broadcasterId, moderatorId, userId, broadcasterId),
            Transport: new Transport(Method: "websocket", SessionId: session));

        using var client = http.CreateTwitchClient();
        using var resp = await client.PostAsJsonAsync("eventsub/subscriptions", body, JsonOptions, cancellationToken);

        var content = await resp.Content.ReadAsStringAsync(cancellationToken);
        if (resp.IsSuccessStatusCode && logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Subscription Reply: {Reply}", content);
        }
        else if (!resp.IsSuccessStatusCode)
        {
            logger.LogError("An error occurred while trying to subscribe to {SubType}: {Content}", type, content);
        }
    }

    internal async Task<GlobalEmoteSet?> GetGlobalEmoteSet()
    {
        using var client = http.CreateTwitchClient();
        client.BaseAddress = TwitchUris.ApiUri;

        return await client.GetFromJsonAsync<GlobalEmoteSet>("chat/emotes/global", JsonOptions);
    }

    internal async Task<ChannelEmoteSet?> GetEmoteSet(string emoteSet)
    {
        using var client = http.CreateTwitchClient();
        client.BaseAddress = TwitchUris.ApiUri;

        return await client.GetFromJsonAsync<ChannelEmoteSet>($"chat/emotes/set?emote_set_id={emoteSet}", JsonOptions);
    }

    internal async Task<ChannelEmoteSet?> GetChannelEmotes(string broadcasterId)
    {
        using var client = http.CreateTwitchClient();
        client.BaseAddress = TwitchUris.ApiUri;

        return await client.GetFromJsonAsync<ChannelEmoteSet>($"chat/emotes?broadcaster_id={broadcasterId}", JsonOptions);
    }

    internal async Task<BadgeSetCollection?> GetGlobalBadgeSet()
    {
        using var client = http.CreateTwitchClient();
        client.BaseAddress = TwitchUris.ApiUri;

        return await client.GetFromJsonAsync<BadgeSetCollection>("chat/badges/global", JsonOptions);
    }

    internal async Task<CheermoteCollection?> GetCheermotes(string? broadcasterId = null)
    {
        using var client = http.CreateTwitchClient();
        client.BaseAddress = TwitchUris.ApiUri;

        return await client.GetFromJsonAsync<CheermoteCollection>($"bits/cheermotes?broadcaster_id={broadcasterId}", JsonOptions);
    }

    internal async Task<BadgeSetCollection?> GetChannelBadgeSet(string broadcasterId)
    {
        using var client = http.CreateTwitchClient();
        client.BaseAddress = TwitchUris.ApiUri;

        return await client.GetFromJsonAsync<BadgeSetCollection>($"chat/badges?broadcaster_id={broadcasterId}", JsonOptions);
    }

    internal async Task<byte[]> GetEmoticon(string emoteId, Format format = Format.Static, Theme theme = Theme.Light, Scale scale = Scale.Medium)
    {
        using var client = http.CreateClient();
        client.BaseAddress = TwitchUris.EmotesUri;

        var type = format switch { Format.Animated => "animated", _ => "static" };
        var mode = theme switch { Theme.Dark => "dark", _ => "light" };
        var size = scale switch { Scale.Medium => "2.0", Scale.Large => "3.0", _ => "1.0" };

        return await client.GetByteArrayAsync($"{emoteId}/{type}/{mode}/{size}");
    }

    internal async Task<byte[]> GetBadge(string badgeId, Scale scale = Scale.Medium)
    {
        using var client = http.CreateClient();
        client.BaseAddress = TwitchUris.BadgesUri;

        var size = scale switch { Scale.Medium => "2", Scale.Large => "3", _ => "1" };

        return await client.GetByteArrayAsync($"{badgeId}/{size}");
    }

    internal async Task SendChatMessage(string broadcasterId, string userId, string message, CancellationToken cancellationToken)
    {
        var body = new
        {
            BroadcasterId = broadcasterId,
            SenderId = userId,
            Message = message
        };

        using var client = http.CreateTwitchClient();
        await client.PostAsJsonAsync("chat/messages", body, JsonOptions, cancellationToken);
    }


    // todo: add purge case when messageId is null
    internal async Task DeleteChatMessage(string broadcasterId, string moderatorId, string messageId, CancellationToken cancellationToken = default)
    {
        using var client = http.CreateTwitchClient();
        await client.DeleteAsync($"moderation/chat?broadcaster_id={broadcasterId}&moderator_id={moderatorId}&message_id={messageId}", cancellationToken);
    }

    internal async Task<(bool Success, string? Message)> BanUser(string broadcasterId, string moderatorId, string userId, TimeSpan? duration = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        object body = duration is { TotalSeconds: var seconds }
            ? new
            {
                Data = new
                {
                    UserId = userId,
                    Duration = (int)seconds,
                    //Reason = reason,
                }
            }
            : new
            {
                Data = new
                {
                    UserId = userId,
                    //Reason = reason,
                }
            };

        using var client = http.CreateTwitchClient();
        var url = $"moderation/bans?broadcaster_id={broadcasterId}&moderator_id={moderatorId}";

        var resp = await client.PostAsJsonAsync(url, body, JsonOptions, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            var text = await resp.Content.ReadAsStringAsync(cancellationToken);
            return (false, text);
        }

        return (true, null);
    }

    internal async Task UnbanUser(string broadcasterId, string moderatorId, string userId, string? reason = null, CancellationToken cancellationToken = default)
    {
        using var client = http.CreateTwitchClient();
        await client.DeleteAsync($"moderation/bans?broadcaster_id={broadcasterId}&moderator_id={moderatorId}&user_id={userId}", cancellationToken);
    }

    internal async Task ResolveUnbanRequest(string broadcasterId, string moderatorId, string unbanRequestId, bool approved, string? reason = null, CancellationToken cancellationToken = default)
    {
        using var client = http.CreateTwitchClient();
        var status = approved ? "approved" : "denied";
        await client.PatchAsync($"moderation/unban_requests?broadcaster_id={broadcasterId}&moderator_id={moderatorId}&unban_request_id={unbanRequestId}&status={status}", null, cancellationToken);
    }
}
