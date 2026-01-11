namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelUnbanRequestCreate : TwitchNotification
{
    public string Id { get; set; } = null!;

    public string UserId { get; init; } = null!;
    public string UserLogin { get; init; } = null!;
    public string UserName { get; init; } = null!;

    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;

    public string? Text { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    internal bool Resolved { get; set; }
}

public class ChannelUnbanRequestResolve : TwitchNotification
{
    public string Id { get; set; } = null!;

    public string UserId { get; init; } = null!;
    public string UserLogin { get; init; } = null!;
    public string UserName { get; init; } = null!;

    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;

    public string ModeratorUserId { get; init; } = null!;
    public string ModeratorUserLogin { get; init; } = null!;
    public string ModeratorUserName { get; init; } = null!;

    public string? ResolutionText { get; init; }
    public string Status { get; init; } = null!;
    internal UnbanResolutionResult ResolutionResult => ResolutionText switch
    {
        "approved" => UnbanResolutionResult.Approved,
        "denied" => UnbanResolutionResult.Denied,
        _ => UnbanResolutionResult.Canceled
    };
}

public enum UnbanResolutionResult { Denied, Canceled, Approved }