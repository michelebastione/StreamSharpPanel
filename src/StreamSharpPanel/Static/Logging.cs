using Serilog;
using Serilog.Filters;

namespace StreamSharpPanel.Static;

internal static class Logging
{
    private const string ChatTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj}{NewLine}";
    private const string DiagnosticsTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}{StackTrace}";

    internal static readonly string ChatPath = Path.Combine(General.AppDataPath, "log", "chat", ".log");
    internal static readonly string DiagnosticsPath = Path.Combine(General.AppDataPath, "log", "diagnostics", ".log");


    private static readonly Serilog.Core.Logger DiagnosticsLogger = new LoggerConfiguration()
        .Filter.ByExcluding(Matching.WithProperty("Chatter"))
        .WriteTo.Console(outputTemplate: DiagnosticsTemplate)
        .WriteTo.File(DiagnosticsPath, outputTemplate: DiagnosticsTemplate, rollingInterval: RollingInterval.Day)
        .CreateLogger();

    private static readonly Serilog.Core.Logger ChatLogger = new LoggerConfiguration()
        .Filter.ByIncludingOnly(Matching.WithProperty("Chatter"))
        .WriteTo.File(ChatPath, outputTemplate: ChatTemplate, rollingInterval: RollingInterval.Day)
        .CreateLogger();

    internal static LoggerConfiguration AddDefaultLogConfiguration(this LoggerConfiguration logger, IConfiguration appConfig) => logger
        .ReadFrom.Configuration(appConfig)
        .Enrich.FromLogContext()
        .WriteTo.Logger(DiagnosticsLogger);

    internal static LoggerConfiguration AddChatLogConfiguration(this LoggerConfiguration logger) => logger.WriteTo.Logger(ChatLogger);
}
