namespace StreamSharpPanel.Models.NotificationEvents;

public class ChannelChatMessage : TwitchNotification
{
    public string ChatterUserId { get; init; } = null!;
    public string ChatterUserLogin { get; init; } = null!;
    public string ChatterUserName { get; init; } = null!;
    public Guid MessageId { get; init; }
    public ChatMessage Message { get; init; } = null!;
    public string? Color { get; init; }

    public string MessageType { get; init; } = null!;
    internal ChatMessageType Type => MessageType switch
    {
        "channel_points_highlighted" => ChatMessageType.ChannelPointsHighlighted,
        "channel_points_sub_only" => ChatMessageType.ChannelPointsSubOnly,
        "user_intro" => ChatMessageType.UserIntro,
        "power_ups_message_effect" => ChatMessageType.PowerUpsMessageEffect,
        "power_ups_gigantified_emote" => ChatMessageType.PowerUpsGigantifiedEmote,
        _ => ChatMessageType.Text
    };

    public Badge[] Badges { get; init; } = [];
    public Cheer? Cheer { get; init; }
    // public MessageReply? Reply { get; init; }

    public string BroadcasterUserId { get; init; } = null!;
    public string BroadcasterUserLogin { get; init; } = null!;
    public string BroadcasterUserName { get; init; } = null!;

    // public string? ChannelPointsCustomRewardId { get; init; }
    // public string? SourceBroadcasterUserId { get; init; }
    // public string? SourceBroadcasterUserLogin { get; init; }
    // public string? SourceBroadcasterUserName { get; init; }
    // public string? SourceMessageId { get; init; }
    // public Badge[]? SourceBadges { get; init; }
    // public bool? IsSourceOnly { get; init; }

    public bool IsDeleted { get; set; }
    public bool ShowDeleted { get; set; }
}

public class Cheer
{
    public int Bits { get; init; }
}

public class Badge
{
    public string SetId { get; init; } = null!;
    public string Id { get; init; } = null!;
    public string Info { get; init; } = null!;
}

public record MessageReply(
    string ParentMessageId,
    string ParentMessageBody,
    string ParentUserId,
    string ParentUserName,
    string ParentUserLogin,
    string ThreadMessageId,
    string ThreadUserId,
    string ThreadUserName,
    string ThreadUserLogin
);

internal enum ChatMessageType 
{ 
    Text,
    ChannelPointsHighlighted,
    ChannelPointsSubOnly,
    UserIntro,
    PowerUpsMessageEffect,
    PowerUpsGigantifiedEmote
}
