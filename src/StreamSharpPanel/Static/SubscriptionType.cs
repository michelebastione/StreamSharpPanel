namespace StreamSharpPanel.Static;


// todo: make the version an integral part of the object?
public static class SubscriptionType
{
    public static class Automod
    {
        public const string MessageHold = "automod.message.hold";
        public const string MessageUpdate = "automod.message.update";
        public const string SettingsUpdate = "automod.settings.update";
        public const string TermsUpdate = "automod.terms.update";
    }
    
    public static class Channel
    {
        public const string Follow = "channel.follow"; // v2
        public const string Update = "channel.update"; // v2
        public const string Ban = "channel.ban";
        public const string Unban = "channel.unban";
        public const string UnbanRequestCreate = "channel.unban_request.create";
        public const string UnbanRequestResolve = "channel.unban_request.resolve";

        public const string Cheer = "channel.cheer";
        public const string Raid = "channel.raid";
        public const string Subscribe = "channel.subscribe";
        public const string SubscriptionGift = "channel.subscription.gift";
        public const string SubscriptionMessage = "channel.subscription.message";

        public const string HypeTrainBegin = "channel.hype_train.begin"; // v2
        public const string HypeTrainProgress = "channel.hype_train.progress"; // v2
        public const string HypeTrainEnd = "channel.hype_train.end"; // v2
        public const string AdBreakBegin = "channel.ad_break.begin";
        
        public const string ChatClear = "channel.chat.clear";
        public const string ChatClearUserMessages = "channel.chat.clear_user_messages";
        public const string ChatMessage = "channel.chat.message";
        public const string ChatMessageDelete = "channel.chat.message_delete";

        public const string ChannelPointsAutomaticRewardRedemption = "channel.channel_points_automatic_reward_redemption.add"; // v2
        public const string ChannelPointsCustomRewardRedemption = "channel.channel_points_custom_reward_redemption.add";
    }

    public static class Stream
    {
        public const string StreamOnline = "stream.online";
        public const string StreamOffline = "stream.offline";
    }
    
    public static class User
    {
        public const string Update = "user.update";
        
        public const string AuthorizationGrant = "user.authorization.grant";
        public const string AuthorizationRevoke = "user.authorization.revoke";
        public const string WhisperMessage = "user.whisper.message";
    }
}