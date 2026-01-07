namespace StreamSharpPanel.Models.Configuration;

public record TwitchSettings
{
    public string ClientId { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string OauthRedirect { get; set; } = null!;
    public string OauthToken { get; set; } = null!;
}