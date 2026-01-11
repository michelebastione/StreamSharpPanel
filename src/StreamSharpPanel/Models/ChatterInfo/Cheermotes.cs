namespace StreamSharpPanel.Models.ChatterInfo;

public class CheermoteCollection
{
    public CheermoteSet[] Data { get; init; } = [];
}

public class CheermoteSet
{
    public string Prefix { get; init; } = null!;
    public Tier[] Tiers { get; init; } = [];
    public string Type { get; init; } = null!;
    internal CheermoteTypes CheermoteType => Type switch
    {
        "global_third_party" => CheermoteTypes.GlobalThirdParty,
        "channel_custom" => CheermoteTypes.ChannelCustom,
        "display_only" => CheermoteTypes.DisplayOnly,
        "sponsored" => CheermoteTypes.Sponsored,
        _ => CheermoteTypes.GlobalFirstParty
    };
    public int Order { get; init; }
    public DateTime LastUpdated { get; init; }
    public bool IsCharitable { get; init; }
}

public class Tier
{
    public int MinBits { get; init; }
    public string Id { get; init; } = null!;
    public string? Color { get; init; }
    public ImageSets Images { get; init; } = new();
    public bool CanCheer { get; init; }
    public bool ShowInBitsCard { get; init; }
}

public class ImageSets
{
    public ImageUrls Dark { get; init; } = new();
    public ImageUrls Light { get; init; } = new();
}

public class ImageUrls
{
    public Dictionary<string, string> Animated { get; init; } = [];
    public Dictionary<string, string> Static { get; init; } = [];
}

public enum CheermoteTypes
{
    GlobalFirstParty,
    GlobalThirdParty,
    ChannelCustom,
    DisplayOnly,
    Sponsored
}