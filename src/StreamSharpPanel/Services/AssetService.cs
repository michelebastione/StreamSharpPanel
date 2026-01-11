using System.Reactive.Linq;
using System.Reactive.Subjects;
using StreamSharpPanel.Models.ChatterInfo;

namespace StreamSharpPanel.Services;

public class AssetService(ILogger<AssetService> logger, ApiCallerService api)
{
    private readonly Subject<AssetsUpdated> _updateStream = new();

    public BadgeSetCollection GlobalBadges { get; private set; } = new();
    public GlobalEmoteSet GlobalEmoticons { get; private set; } = new();
    public CheermoteCollection GlobalCheermotes { get; private set; } = new();

    public Dictionary<string, BadgeSetCollection> ChannelBadges { get; private set; } = [];
    public Dictionary<string, ChannelEmoteSet> ChannelEmoticons { get; private set; } = [];


    public async Task<bool> UpdateGlobalAssetsUrls()
    {
        try
        {
            GlobalBadges = await api.GetGlobalBadgeSet() ?? new();
            GlobalEmoticons = await api.GetGlobalEmoteSet() ?? new();
            GlobalCheermotes = await api.GetCheermotes() ?? new();
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
