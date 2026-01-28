using System.Text.Json.Serialization;
using StreamSharpPanel.Models.Http;
using StreamSharpPanel.Static;

namespace StreamSharpPanel.Models.ChatterInfo;

public class GlobalEmoteSet
{
    public EmoteInfo[] Data { get; set; } = [];
    
    internal Dictionary<string, string?> Urls
    {
        get
        {
            if (Data is [])
                return [];

            if (field.Count == 0)
                field = Data.ToDictionary(e => e.Id, e => e.Images?.SmallImgUrl);

            return field;
        }
    } = [];

    public string Template { get; set; } = null!;
}

public class EmoteInfo
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ImageSet? Images { get; set; }

    public string[] Format { get; set; } = [];
    internal General.Format[] Types => Format
        .Select(f => f switch { "animated" => General.Format.Animated, _ => General.Format.Static })
        .ToArray();

    public string[] Scale { get; set; } = [];
    internal General.Scale[] Sizes => Scale
        .Select(s => s switch { "1.0" => General.Scale.Small, "3.0" => General.Scale.Large, _ => General.Scale.Medium })
        .ToArray();

    public string[] ThemeMode { get; set; } = [];
    internal General.Theme[] Modes => ThemeMode
        .Select(t => t switch { "dark" => General.Theme.Dark, _ => General.Theme.Light })
        .ToArray();
}

public class ImageSet
{
    [JsonPropertyName("url_1x")] public string? SmallImgUrl { get; set; }
    [JsonPropertyName("url_2x")] public string? MediumImgUrl { get; set; }
    [JsonPropertyName("url_4x")] public string? LargeImgUrl { get; set; }
}


public class ChannelEmoteSet
{
    public ChannelEmoteInfo[] Data { get; set; } = [];

    internal Dictionary<string, string?> Urls
    {
        get
        {
            if (Data is [])
                return [];

            if (field.Count == 0)
                field = Data.ToDictionary(e => e.Id, e => e.Images?.SmallImgUrl);

            return field;
        }
    } = [];

    public string Template { get; set; } = null!;
}

public class ChannelEmoteInfo : EmoteInfo
{
    public string EmoteSetId { get; set; } = null!;
    public string Tier { get; set; } = null!;
    
    public string EmoteType { get; set; } = null!;
    internal ChannelEmoteType? Type => EmoteType switch
    {
        "bitstier" => ChannelEmoteType.BitsTier,
        "follower" => ChannelEmoteType.Follower,
        "subscriptions" => ChannelEmoteType.Subscriptions,
        _ => null
    };
}

public class UserEmoteSet
{
    public UserEmoteInfo[] Data { get; set; } = [];
    public string Template { get; set; } = null!;
    public Pagination? Pagination { get; init; }
}

public class UserEmoteInfo : ChannelEmoteInfo
{
    public string OwnerId { get; set; } = null!;
    internal string StaticUrl => $"{TwitchUris.EmotesUri}{Id}/static/light/1.0";
    internal string AnimatedUrl => $"{TwitchUris.EmotesUri}{Id}/animated/light/1.0";
}


internal enum ChannelEmoteType 
{
    None,
    BitsTier, 
    Follower, 
    Subscriptions,
    ChannelPoints,
    Rewards,
    HypeTrain,
    Prime,
    Turbo,
    Smilies,
    Globals,
    Owl2019,
    TwoFactor,
    LimitedTime
}
