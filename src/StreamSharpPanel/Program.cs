using Serilog;
using StreamSharpPanel.Services;
using StreamSharpPanel.Components;
using StreamSharpPanel.Static;


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Path.GetDirectoryName(Environment.ProcessPath)
});

Log.Logger = new LoggerConfiguration()
    .AddDefaultLogConfiguration(builder.Configuration)
    .CreateBootstrapLogger()
    .ForContext("SourceContext", builder.Environment.ApplicationName);

Log.Information("{Application} is starting...", builder.Environment.ApplicationName);

try
{
    Directory.CreateDirectory(Path.GetDirectoryName(Logging.DiagnosticsPath)!);
    Directory.CreateDirectory(Path.GetDirectoryName(Logging.ChatPath)!);

    var httpPort = builder.Configuration.GetValue<int?>("HttpPort") ?? General.DefaultHttpPort;

    builder.WebHost.ConfigureKestrel(opts =>
    {
        opts.ListenLocalhost(httpPort);
    });

    builder.Services.AddRazorComponents().AddInteractiveServerComponents();
    builder.Services.AddMudBlazor();

    builder.Services.AddSerilog((services, logger) => logger
        .ReadFrom.Services(services)
        .AddDefaultLogConfiguration(builder.Configuration)
        .AddChatLogConfiguration()); // we use a separate sink to log chat messages

    builder.Services
        .AddTwitchHttpClients(builder.Configuration)
        .AddDataProtection();

    builder.Services
        .AddHostedSingleton<SettingsService>()
        .AddSingleton<AuthService>()
        .AddSingleton<SettingsService>()
        .AddSingleton<EventSubService>()
        .AddSingleton<ApiCallerService>()
        .AddSingleton<AssetService>()
        .AddScoped<StateService>();

    var app = builder.Build();
        
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
    }

    app.LogMessageOnStartup(logger =>
        logger.LogInformation("{Application} is running on {Url}", app.Environment.ApplicationName, $"http://localhost:{httpPort}"));

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
