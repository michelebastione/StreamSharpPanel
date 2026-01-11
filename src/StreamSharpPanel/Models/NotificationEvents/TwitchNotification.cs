namespace StreamSharpPanel.Models.NotificationEvents;

public abstract class TwitchNotification
{
    public virtual string Format() => "";
}

public class UnhandledNotification : TwitchNotification;

public class MessageNotification
{
    public string Text { get; init; } = null!;
    public MessageFragment[] Fragments { get; init; } = [];
}

public class MessageFragment
{
    public string Type { get; init; } = null!;
    public MessageFragmentType FragmentType => Type switch
    {
        "cheermote" => MessageFragmentType.Cheermote,
        "emote" => MessageFragmentType.Emote,
        "mention" => MessageFragmentType.Mention,
        _ => MessageFragmentType.Text
    };

    public string Text { get; init; } = null!;
    public Cheermote? Cheermote { get; set; }
    public Emote? Emote { get; set; }
    public Mention? Mention { get; set; }
}

public record Cheermote(string Prefix, int Bits, int Tier);
public record Emote(string Id, string EmoteSetId, string OwnerId, string[] Format);
public record Mention(string UserId, string UserName, string UserLogin);

public enum MessageFragmentType { Text, Cheermote, Emote, Mention }