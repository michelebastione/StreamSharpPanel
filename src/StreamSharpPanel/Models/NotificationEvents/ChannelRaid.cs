namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelRaid : ITwitchNotification
{
    public string FromBroadcasterUserId { get; init; } = null!;
    public string FromBroadcasterUserLogin { get; init; } = null!;
    public string FromBroadcasterUserName { get; init; } = null!;

    public string ToBroadcasterUserId { get; init; } = null!;
    public string ToBroadcasterUserLogin { get; init; } = null!;
    public string ToBroadcasterUserName { get; init; } = null!;

    public int Viewers { get; init; }
}