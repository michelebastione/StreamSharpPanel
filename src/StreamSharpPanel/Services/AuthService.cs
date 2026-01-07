using Microsoft.AspNetCore.Http.Extensions;
using StreamSharpPanel.Static;
using System.Net.Http.Headers;
using StreamSharpPanel.Models.Http;
using static StreamSharpPanel.Static.General;

namespace StreamSharpPanel.Services;

public class AuthService(ILogger<AuthService> logger, IHttpClientFactory http)
{
    private const string OauthRedirect = "http://localhost:3000/oauth/callback";
    private const string AuthorizeUrlPath = "oauth2/authorize";
    private const string ValidateUrlPath = "oauth2/validate";

    private static QueryBuilder CreateAuthQuery(string clientId) => new()
    {
        { "client_id", clientId },
        { "redirect_uri", OauthRedirect },
        { "response_type", "token" },
        { "scope", string.Join(' ',
            [
                AuthScopes.Bits.Read,
                //AuthScopes.Clips.Edit
                AuthScopes.Channel.ManageBroadcast,
                AuthScopes.Channel.ReadRedemptions,
                AuthScopes.Channel.ReadSubscriptions,
                AuthScopes.Channel.Moderate,
                AuthScopes.Moderation.Read,
                AuthScopes.Moderator.ManageAutomod,
                AuthScopes.Moderator.ManageAnnouncements,
                AuthScopes.Moderator.ManageBannedUsers,
                AuthScopes.Moderator.ManageChatMessages,
                AuthScopes.Moderator.ReadChatters,
                AuthScopes.Moderator.ReadFollowers,
                AuthScopes.Moderator.ReadSuspiciousUsers,
                AuthScopes.User.ReadEmotes,
                AuthScopes.User.ReadChat,
                AuthScopes.User.WriteChat
            ])
        }
    };

    internal string BuildTokenRequestUrl(string clientId)
    {
        var uri = new UriBuilder(TwitchUris.AuthUri)
        {
            Path = AuthorizeUrlPath,
            Query = CreateAuthQuery(clientId).ToString()
        };

        return uri.ToString();
    }

    internal async Task<(bool Success, string? Message, ValidTokenResponse? Result)> ValidateToken(string token)
    {
        using var client = http.CreateClient("twitch-auth");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        using var response = await client.GetAsync(ValidateUrlPath);
        if (!response.IsSuccessStatusCode)
        {
            var failedValidationResponse = await response.Content.ReadFromJsonAsync<InvalidTokenResponse>();
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("An error occurred during token validation - Status: {Status}, Error: {Err}", failedValidationResponse?.Status ?? default, failedValidationResponse?.Message ?? "Validation Error");
            }
            return (false, failedValidationResponse?.Message ?? "Validation Error", null);
        }

        var okValidationResponse = await response.Content.ReadFromJsonAsync<ValidTokenResponse>(JsonOptions);
        return (true, null, okValidationResponse);
    }
}
