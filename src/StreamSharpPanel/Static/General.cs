using MudBlazor;
using MudBlazor.Utilities;
using System.Text.Json;

namespace StreamSharpPanel.Static;

internal static class General
{
    internal const int DefaultHttpPort = 3000;
    internal const string DefaultClientId = "6rrlrjqkrjd2v5a7kz8wo173yv3ixw";

    internal static readonly string AppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        AppDomain.CurrentDomain.FriendlyName);
    
    internal static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy =  JsonNamingPolicy.SnakeCaseLower
    };

    internal static readonly MudTheme MainTheme = new()
    {
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "sans-serif"]
            }
        },

        PaletteLight = new PaletteLight
        {
            Primary = new MudColor("#772ce8"),
        },

        PaletteDark = new PaletteDark
        {
            Primary = new MudColor("#9747ff"),
            Secondary = new MudColor("#ae1392")
        }
    };

    internal enum Theme { Light, Dark }
    internal enum Format { Static, Animated }
    internal enum Scale { Small, Medium, Large }
}
