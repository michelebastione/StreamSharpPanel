namespace StreamSharpPanel.Models;

public class MessageMetadata
{
    public required string MessageId { get; init; }
    public string MessageType { get; init; } =  null!;
    public DateTimeOffset MessageTimestamp { get; init; }
}

public class TwitchSession
{
    public required string Id { get; init; }
    public string? Status { get; init; }
    public DateTimeOffset ConnectedAt { get; init; }
    public int? KeepaliveTimeoutSeconds { get; init; }
    public string? ReconnectUrl { get; init; }
}

public class Subscription
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Version { get; set; } = null!;
    public int Cost { get; set; }
    public Condition? Condition { get; set; }
    public Transport? Transport { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public record SubscriptionRequest(string Type, string Version, Condition Condition, Transport Transport);

// todo: different subscription types take different condition properties
// what's the cleanest way to solve the serialization of derived types?
// should we make a custom JsonConverter? Or should we use a different approach entirely?
// temp ugly workaround: adding all properties to a single type
public record Condition(string BroadcasterUserId, string? ModeratorUserId = null, string? UserId = null, string? ToBroadcasterUserId = null);
public record Transport(string Method, string SessionId);
