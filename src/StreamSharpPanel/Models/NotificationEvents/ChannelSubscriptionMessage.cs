using System.Text;

namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelSubscriptionMessage : TwitchNotification
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

    public override string Format()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("{0} resubscribed", UserName);
        
        if (StreakMonths is int months)
        {
            sb.Append(months);
        }
        
        sb.Append('!');
        if (Message?.Text is { } msg)
        {
            sb.AppendFormat(" They say: \"{0}\"", msg);
        }

        return sb.ToString();
    }
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