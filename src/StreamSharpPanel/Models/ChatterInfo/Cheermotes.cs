namespace StreamSharpPanel.Models.ChatterInfo;

public class CheermoteCollection
{
    public CheermoteSet[] Data { get; init; } = [];
}

public class CheermoteSet
{
    public string Prefix { get; init; } = null!;
    public CheermoteTier[] Tiers { get; init; } = [];
    public string Type { get; init; } = null!;
    internal CheermoteTypes CheermoteType => Type switch
    {
        "global_first_party" => CheermoteTypes.GlobalFirstParty,
        "global_third_party" => CheermoteTypes.GlobalThirdParty,
        "channel_custom" => CheermoteTypes.ChannelCustom,
        "display_only" => CheermoteTypes.DisplayOnly,
        "sponsored" => CheermoteTypes.Sponsored,
        _ => CheermoteTypes.Unknown
    };
    public int Order { get; init; }
    public DateTime LastUpdated { get; init; }
    public bool IsCharitable { get; init; }

    internal Dictionary<string, CheermoteTier> CheermoteTiers
    {
        get
        {
            if (Tiers is [])
                return [];

            if (field.Count == 0)
                field = Tiers.ToDictionary(t => t.Id, t => t);

            return field;
        }
    } = [];
}

public class CheermoteTier
{
    public int MinBits { get; init; }
    public string Id { get; init; } = null!;
    public string? Color { get; init; }
    public CheermoteImages Images { get; init; } = new();
    public bool CanCheer { get; init; }
    public bool ShowInBitsCard { get; init; }
}

public class CheermoteImages
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
    Unknown,
    GlobalFirstParty,
    GlobalThirdParty,
    ChannelCustom,
    DisplayOnly,
    Sponsored
}