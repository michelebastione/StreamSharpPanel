namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelSubscriptionMessage : ITwitchNotification
{
    public string UserId { get; init; } = null!;
    public string UserLogin { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;

    public string Tier { get; init; } = null!;
    public SubscriptionTier SubTier => Tier switch
    {
        "2000" => SubscriptionTier.Tier2,
        "3000" => SubscriptionTier.Tier3,
        _ => SubscriptionTier.Tier1,
    };

    public SubscriptionMessage? Message { get; init; }

    public int CumulativeMonths { get; init; }
    public int? StreakMonths { get; init; }
    public int DurationMonths { get; init; }
}

public class SubscriptionMessage
{
    public string? Text { get; init; }
    public ResubscriptionEmote[] Emotes { get; init; } = [];
}

public class ResubscriptionEmote
{
    public int Begin { get; init; }
    public int End { get; init; }
    public string Id { get; init; } = null!;
}