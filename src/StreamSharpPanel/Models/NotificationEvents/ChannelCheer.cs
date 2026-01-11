namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelCheer : TwitchNotification
{
    public bool IsAnonymous { get; init; }

    public string? UserId { get; init; }
    public string? UserLogin { get; init; }
    public string? UserName { get; init; }

    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;

    public string? Message { get; init; }
    public int Bits { get; init; }
}