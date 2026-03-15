using System.Text.Json.Serialization;

namespace StreamSharpPanel.Models.Http;

public class GetChatterResponse
{
    public TwitchChatter[] Data { get; init; } = [];
    public TwitchPagination? Pagination { get; init; }
    public int Total { get; init; }
}

public class TwitchChatter
{
    public string UserId { get; init; } = null!;
    public string UserLogin { get; init; } = null!;
    public string UserName { get; init; } = null!;
}

public enum ChatterType
{
    Broadcaster,
    Moderator,
    Vip,
    Bot,
    Chatter
}
