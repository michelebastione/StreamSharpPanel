using System.Text.Json.Serialization;

namespace StreamSharpPanel.Models.ChatterInfo;

public class BadgeSetCollection
{
    public BadgeSet[] Data { get; set; } = [];

    internal Dictionary<string, BadgeSet> BadgeSets
    {
        get
        {
            if (Data is [])
                return [];

            if (field.Count == 0)
                field = Data.ToDictionary(b => b.SetId, b => b);

            return field;
        }
    } = [];

    internal BadgeSet? GetBadgeSet(string setId) => BadgeSets.GetValueOrDefault(setId);
}

public class BadgeSet
{
    public string SetId { get; set; } = null!;
    public BadgeInfo[] Versions { get; set; } = [];

    internal Dictionary<string, BadgeInfo> BadgeVersions
    {
        get
        {
            if (Versions is [])
                return [];

            if (field.Count == 0)
                field = Versions.ToDictionary(b => b.Id, b => b);

            return field;
        }
    } = [];

    internal BadgeInfo? GetBadgeInfo(string versionId) => BadgeVersions.GetValueOrDefault(versionId);

}

public class BadgeInfo
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    
    public string? ClickAction { get; set; }
    public string? ClickUrl { get; set; }

    [JsonPropertyName("image_url_1x")] public string? Url1x { get; set; }
    [JsonPropertyName("image_url_2x")] public string? Url2x { get; set; }
    [JsonPropertyName("image_url_4x")] public string? Url4x { get; set; }
}
