using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;
using StreamSharpPanel.Models.Configuration;
using StreamSharpPanel.Static;

namespace StreamSharpPanel.Services;

public class SettingsService(IDataProtectionProvider protection, IConfiguration config) : IHostedService
{
    private static readonly string SettingsPath = Path.Combine(General.AppDataPath, "data");

    private readonly IDataProtector _dataProtector = protection.CreateProtector("settings");

    internal readonly int ServerHttpPort = config.GetValue<int?>("HttpPort") ?? General.DefaultHttpPort;

    internal TwitchSettings CurrentSettings { get; private set; } = new();
    internal Action? OnSettingsUpdated { get; set; }


    internal async Task<TwitchSettings?> LoadSettings()
    {
        if (!File.Exists(SettingsPath))
            return null;

        var text = await File.ReadAllTextAsync(SettingsPath);
        var json = _dataProtector.Unprotect(text);

        return JsonSerializer.Deserialize<TwitchSettings>(json);
    }

    internal async Task SaveSettings(TwitchSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        var data = _dataProtector.Protect(json);
        await File.WriteAllTextAsync(SettingsPath, data);

        CurrentSettings = settings;
        OnSettingsUpdated?.Invoke();
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var clientId = config.GetValue<string>("ClientId");
        var clientIdValue = !string.IsNullOrWhiteSpace(clientId) 
            ? clientId 
            : General.DefaultClientId;

        CurrentSettings = await LoadSettings()
            ?? new TwitchSettings { ClientId = clientIdValue };
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
