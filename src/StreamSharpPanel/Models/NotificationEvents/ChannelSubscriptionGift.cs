namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelSubscriptionGift : ITwitchNotification
{
    public string UserId { get; init; } = null!;
    public string UserLogin { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;
    public int Total { get; init; }

    public string Tier { get; init; } = null!;
    public SubscriptionTier SubTier => Tier switch
    {
        "2000" => SubscriptionTier.Tier2,
        "3000" => SubscriptionTier.Tier3,
        _ => SubscriptionTier.Tier1,
    };

    public int? CumulativeTotal { get; init; }
    public bool IsAnonymous { get; init; }
}

