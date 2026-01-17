namespace StreamSharpPanel.Models.NotificationEvents;

public class AutomodMessageHold : TwitchNotification
{
    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;

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

    internal bool Resolved { get; set; }
}

public enum BlockedReason { Automod, BlockedTerm }

public class Automod
{
    public string? Category { get; init; }
    public int Level { get; init; }
    public Boundary[] Boundaries { get; init; } = [];
}

public class Boundary
{
    public int StartPos { get; init; }
    public int EndPos { get; init; }
}

public class BlockedTermsContainer
{
    public BlockedTerm[] TermsFound { get; init; } = [];
}

public class BlockedTerm
{
    public string TermId { get; init; } = null!;
    public Boundary Boundary { get; init; } = null!;
    public string OwnerBroadcasterUserId { get; init; } = null!;
    public string OwnerBroadcasterUserLogin { get; init; } = null!;
    public string OwnerBroadcasterUserName { get; init; } = null!;
}
