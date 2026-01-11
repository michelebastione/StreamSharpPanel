namespace StreamSharpPanel.Static;

internal static class TwitchUris
{
    internal static readonly Uri AuthUri = new UriBuilder
    {
        Scheme = "https",
        Host = "id.twitch.tv",
        Port = 443
    }.Uri;

    //internal static readonly Uri EventSubUri = new UriBuilder("ws", "localhost", 8080, "ws").Uri;

    internal static readonly Uri EventSubUri = new UriBuilder
    {
        Scheme = "wss",
        Host = "eventsub.wss.twitch.tv",
        Port = 443,
        Path = "ws"
    }.Uri;

    //internal static readonly Uri ApiUri = new UriBuilder("http", "localhost", 8080).Uri;
    internal static readonly Uri ApiUri = new UriBuilder
    {
        Scheme = "https",
        Host = "api.twitch.tv",
        Port = 443,
        Path = "helix/"
    }.Uri;

    internal static readonly Uri EmotesUri = new UriBuilder
    {
        Scheme = "https",
        Host = "static-cdn.jtvnw.net",
        Port = 443,
        Path = "emoticons/v2/"
    }.Uri;
    
    internal static readonly Uri BadgesUri = new UriBuilder
    {
        Scheme = "https",
        Host = "static-cdn.jtvnw.net",
        Port = 443,
        Path = "badges/v1/"
    }.Uri;
}