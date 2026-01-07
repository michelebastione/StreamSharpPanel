using StreamSharpPanel.Models.NotificationEvents;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace StreamSharpPanel.Static;

internal static class Extensions
{
    internal static void LogMessageOnStartup(this IHost app, Action<ILogger> loggerCallback)
    {
        var appLogger = app.Services
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(AppDomain.CurrentDomain.FriendlyName);

        app.Services
            .GetRequiredService<IHostApplicationLifetime>()
            .ApplicationStarted.Register(() => loggerCallback.Invoke(appLogger));
    }

    internal static void LogChatMessage(this ILogger logger, ChannelChatMessage message) =>
        logger.LogInformation("{Chatter}: {Message}", message.ChatterUserName, message.Message.Text);

    internal static IServiceCollection AddHostedSingleton<T>(this IServiceCollection services)
        where T : class, IHostedService => services
            .AddSingleton<T>()
            .AddHostedService(serviceProvider => serviceProvider.GetRequiredService<T>());

    internal static HttpClient CreateTwitchClient(this IHttpClientFactory http) => http.CreateClient("twitch-api");

    extension(Task)
    {
        public static async Task NullSafeInvoke(Task? task) => await (task ?? Task.CompletedTask);

        public static async Task<T?> NullSafeInvoke<T>(Task<T?>? task) => await (task ?? Task.FromResult<T?>(default));
    }
}
