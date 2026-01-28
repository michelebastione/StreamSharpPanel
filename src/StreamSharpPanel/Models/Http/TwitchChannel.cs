namespace StreamSharpPanel.Models.Http;

public class TwitchChannelSearchResult
{
    public TwitchChannelData[] Data { get; init; } = [];
    public Pagination? Pagination { get; init; }
}

public class TwitchChannelData
{
    public string? BroadcasterLanguage { get; init; }

    public string Id { get; init; } = null!;
    public string BroadcasterLogin { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    
    public string? GameId { get; init; }
    public string? GameName { get; init; }
    
    public bool IsLive { get; init; }

    public string[] TagIds { get; init; } = []; // Deprecated field, use Tags instead
    public string[] Tags { get; init; } = [];
    
    public string? ThumbnailUrl { get; init; }
    public string? Title { get; init; }
    
    public DateTimeOffset StartedAt { get; init; }
}

public class Pagination
{
    public string? Cursor { get; init; }
}
