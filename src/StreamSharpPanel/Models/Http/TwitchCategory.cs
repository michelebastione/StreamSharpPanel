namespace StreamSharpPanel.Models.Http;

public class TwitchCategorySearchResult
{
    public TwitchCategory[] Data { get; init; } = [];
}

public class TwitchCategory
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string BoxArtUrl { get; init; } = null!;
    public string IgdbId { get; init; } = "";
}
