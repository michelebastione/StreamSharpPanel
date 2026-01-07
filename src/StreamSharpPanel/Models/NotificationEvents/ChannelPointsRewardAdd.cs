namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelPointsCustomRewardAdd : ITwitchNotification
{
    public string? Id { get; set; }
    public string BroadcasterUserId { get; set; } = null!;
    public string BroadcasterUserName { get; set; } = null!;
    public string BroadcasterUserLogin { get; set; } = null!;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserLogin { get; set; }
    public string? UserInput { get; set; }
    public string? Status { get; set; }
    public Reward? Reward { get; set; }
    //public SubscriptionReplyMessage? Message { get; set; }
    public DateTime RedeemedAt { get; set; }
}

public class Reward
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Prompt { get; set; }
    public int Cost { get; set; }
}
