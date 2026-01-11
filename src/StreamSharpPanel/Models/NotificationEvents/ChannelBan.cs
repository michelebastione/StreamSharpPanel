namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelBan : TwitchNotification
{
    public string UserId { get; init; } = null!;
    public string UserLogin { get; init; } = null!;
    public string UserName { get; init; } = null!;

    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;
    
    public string ModeratorUserId { get; init; } = null!;
    public string ModeratorUserLogin { get; init; } = null!;
    public string ModeratorUserName { get; init; } = null!;
    
    public string? Reason { get; init; }
    
    public DateTimeOffset BannedAt { get; init; }
    public DateTimeOffset? EndsAt { get; init; }
    
    public bool IsPermanent { get; init; }
}
