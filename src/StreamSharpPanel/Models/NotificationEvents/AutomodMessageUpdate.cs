using System.Diagnostics;

namespace StreamSharpPanel.Models.NotificationEvents;

public class AutomodMessageUpdate : TwitchNotification
{
    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;
    
    public string ModeratorUserId { get; init; } = null!;
    public string ModeratorUserLogin { get; init; } = null!;
    public string ModeratorUserName { get; init; } = null!;

    public string UserId { get; init; } = null!;
    public string UserLogin { get; init; } = null!;
    public string UserName { get; init; } = null!;
    
    public string MessageId { get; init; } = null!;
    public ChatMessage Message { get; init; } = null!;

    public string? Reason { get; init; }
    public BlockedReason BlockedReason => Reason switch
    {
        "automod" => BlockedReason.Automod,
        "blocked_term" => BlockedReason.BlockedTerm,
        _ => default
    };

    public Automod? Automod { get; init; }
    public BlockedTermsContainer? BlockedTerm { get; init; }

    public DateTimeOffset HeldAt { get; init; }
    public string Status { get; init; } = null!;
    public AutomodMessageStatus MessageStatus => Status switch
    {
        "Approved" => AutomodMessageStatus.Approved,
        "Denied" => AutomodMessageStatus.Denied,
        "Expired" => AutomodMessageStatus.Expired,
        _ => throw new UnreachableException($"Unknown automod message status: {Status}")
    };
}

public enum AutomodMessageStatus
{
    Approved,
    Denied,
    Expired
}
