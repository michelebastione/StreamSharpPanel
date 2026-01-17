using System.Net.Http.Headers;
using MudBlazor;
using MudBlazor.Services;
using StreamSharpPanel.Services;

namespace StreamSharpPanel.Static;

internal static class DependencyInjection
{
    internal static IServiceCollection AddTwitchHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("twitch-api", async (services, client) =>
        {
            var settings = services.GetRequiredService<SettingsService>();

            client.BaseAddress = TwitchUris.ApiUri;
            client.DefaultRequestHeaders.Add("Client-Id", settings.CurrentSettings.ClientId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.CurrentSettings.OauthToken);
        });

        services.AddHttpClient("twitch-auth", client =>
        {
            client.BaseAddress = TwitchUris.AuthUri;
        });

        return services;
    }

    internal static IServiceCollection AddMudBlazor(this IServiceCollection services)
    {
        return services.AddMudServices(conf =>
            conf.SnackbarConfiguration = new SnackbarConfiguration
            {
                VisibleStateDuration = 3500,
                ShowTransitionDuration = 350,
                HideTransitionDuration = 350,
            });
    }
}
