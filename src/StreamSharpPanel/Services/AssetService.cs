using System.Reactive.Linq;
using System.Reactive.Subjects;
using StreamSharpPanel.Models.ChatterInfo;
using StreamSharpPanel.Models.Http;

namespace StreamSharpPanel.Services;

public class AssetService(ILogger<AssetService> logger, ApiCallerService api)
{
    private readonly Subject<AssetsUpdated> _updateStream = new();

    public BadgeSetCollection GlobalBadges { get; private set; } = new();
    public GlobalEmoteSet GlobalEmoticons { get; private set; } = new();

    public Dictionary<string, CheermoteSet> Cheermotes { get; private set; } = [];
    public Dictionary<string, BadgeSetCollection> ChannelBadges { get; private set; } = [];
    public Dictionary<string, ChannelEmoteSet> ChannelEmoticons { get; private set; } = [];
    public ILookup<TwitchUser, UserEmoteInfo>? UserEmotes { get; private set; }

    internal BadgeInfo? GetChatterBadge(ChatterType type)
    {
        var setId = type switch
        {
            ChatterType.Broadcaster => "broadcaster",
            ChatterType.Moderator => "moderator",
            ChatterType.Vip => "vip",
            ChatterType.Bot => "bot-badge",
            _ => null
        };

        return GlobalBadges.Data.FirstOrDefault(e => e.SetId == setId)?.Versions[0];
    }

    public async Task<bool> UpdateGlobalAssetsUrls()
    {
        try
        {
            GlobalBadges = await api.GetGlobalBadgeSet() ?? new();
            GlobalEmoticons = await api.GetGlobalEmoteSet() ?? new();

            _updateStream.OnNext(new BadgesUpdated());
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching global assets: ");
            return false;
        }
    }

    public async Task<bool> UpdateChannelBadgesUrls(string broadcasterId)
    {
        try
        {
            if (await api.GetChannelBadgeSet(broadcasterId) is { } badges)
            {
                ChannelBadges[broadcasterId] = badges;
                _updateStream.OnNext(new BadgesUpdated());
                return true;
            }
            else if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("No badges found for user {User}", broadcasterId);
            }
            
            return false;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching badges of user {User}: ", broadcasterId);    
            return false;
        }
    }

    public BadgeInfo? GetChannelBadgeOrDefault(string broadcasterId, string setId, string version)
    {
        return ChannelBadges.GetValueOrDefault(broadcasterId)
            ?.GetBadgeSet(setId)
            ?.GetBadgeInfo(version) 

        ?? GlobalBadges
            ?.GetBadgeSet(setId)?.GetBadgeInfo(version);
    }

    public async Task<bool> UpdateChannelEmotesUrls(string setId)
    {
        try
        {
            if (await api.GetEmoteSet(setId) is { } emoteSet)
            {
                ChannelEmoticons[setId] = emoteSet;
                _updateStream.OnNext(new EmotesUpdated());
                return true;
            }
            else if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("No badges found for user {User}", setId);
            }
            
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching badges of user {User}: ", setId);    
            return false;
        }
    }

    public async Task<bool> UpdateUserEmotesUrls(string setId)
    {
        try
        {
            var emotes = await api.GetAllUserEmotes(setId);

            var infos = await api.GetUsersInfo(ids: emotes.Select(e => e.OwnerId));
            var infoDict = infos?.Data.ToDictionary(u => u.Id, u => u) ?? [];

            var globalUser = new TwitchUser
            {
                Id = "",
                Login = "",
                DisplayName = "Global"
            };

            var unlockedUser = new TwitchUser
            {
                Id = "",
                Login = "",
                DisplayName = "Unlocked"
            };

            UserEmotes = emotes.ToLookup(g => g.OwnerId switch 
            {
                "" => globalUser,
                "twitch" => unlockedUser,
                _ => infoDict.TryGetValue(g.OwnerId, out var user) ? user : new() { Id = "", Login = "", DisplayName = g.OwnerId }
            });

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching badges of user {User}: ", setId);    
            return false;
        }
    }

    internal async Task UpdateCheermotes(string? broadcasterId = null)
    {
        var cheermotes = await api.GetCheermotes(broadcasterId);
        foreach(var set in cheermotes?.Data ?? [])
        {
            Cheermotes.TryAdd(set.Prefix, set);
        }
    }

    /// <summary>
    /// Retrieves a specific cheermote's animated and static URLs
    /// </summary>
    /// <param name="prefix">The id of the Cheermote Set</param>
    /// <param name="tier">The id of the Cheermote Type</param>
    /// <param name="darkMode">Whether the dashboard is set to display dark mode</param>
    /// <param name="size">Size of the cheermote. Can only be one of "1", "1.5", "2", "3" and "4"</param>
    /// <returns></returns>
    internal (string? Animated, string? Static) TryGetCheermoteUrl(string prefix, string tier, bool darkMode = false, string size = "1")
    {
        var cheermoteSet = Cheermotes.GetValueOrDefault(prefix);
        var cheermoteTier= cheermoteSet?.CheermoteTiers.GetValueOrDefault(tier);

        var images = darkMode 
            ? cheermoteTier?.Images.Dark 
            : cheermoteTier?.Images.Light;

        return (images?.Animated.GetValueOrDefault(size),
            images?.Static.GetValueOrDefault(size));
    }

    internal IDisposable OnAssetsUpdated<T>(Action<T> callback) where T : AssetsUpdated, new()
    {
        return _updateStream
            .OfType<T>()
            .Subscribe(callback);
    }
}

internal abstract class AssetsUpdated;
internal class BadgesUpdated : AssetsUpdated;
internal class EmotesUpdated : AssetsUpdated;
