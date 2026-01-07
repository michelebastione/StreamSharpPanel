using System.Text.Json.Nodes;

namespace StreamSharpPanel.Models;

public interface IMessagePayload;

public class KeepAlivePayload : IMessagePayload;

public class WelcomePayload : IMessagePayload
{
    public TwitchSession Session { get; init; } = null!;
}

public class NotificationPayload : IMessagePayload
{
    public Subscription Subscription { get; init; } = null!;
    public JsonNode Event { get; init; } = null!;
}

public class ReconnectPayload : IMessagePayload
{
    public TwitchSession Session { get; init; } = null!;
}

public class RevocationPayload : IMessagePayload
{
    public Subscription Subscription { get; init; } = null!;
    public Transport Transport { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
}