namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelChatMessageDelete : TwitchNotification
{
    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string TargetUserId { get; init; } = null!;
    public string TargetUserName { get; init; } = null!;
    public string TargetUserLogin { get; init; } = null!;
    public string MessageId { get; init; } = null!;
}
