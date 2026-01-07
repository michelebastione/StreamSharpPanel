namespace StreamSharpPanel.Models.Http;

public class GetUserResponse
{
    public TwitchUser[] Data { get; init; } = [];
}

public class TwitchUser
{
    public required string Id { get; init; }
    public required string Login { get; init; }
    public string DisplayName { get; init; } = null!;
    public string Type { get; init; } = null!;
    public string? BroadcasterType { get; init; }
    public string? Description { get; init; }
    public int ViewCount { get; init; }
    public DateTime CreatedAt { get; init; }

    public string? ProfileImageUrl { get; init; }
    public string? OfflineImageUrl { get; init; }
}