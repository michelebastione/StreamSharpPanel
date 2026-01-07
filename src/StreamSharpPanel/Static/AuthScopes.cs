namespace StreamSharpPanel.Static;

internal static class AuthScopes
{
    internal static class Bits
    {
        internal const string Read = "bits:read";
    }

    internal static class Channel
    {
        internal const string Moderate = "channel:moderate";
        internal const string ManageBroadcast = "channel:manage:broadcast";
        internal const string ManagePredictions = "channel:manage:predictions";
        internal const string ManageRaids = "channel:manage:raids";
        internal const string ManageVips = "channel:manage:vips";
        internal const string ReadRedemptions = "channel:read:redemptions";
        internal const string ManageRedemptions = "channel:manage:redemptions";
        internal const string ReadSubscriptions = "channel:read:subscriptions";
    }

    internal static class Clips
    {
        internal const string Edit = "clips:edit";
    }

    internal static class Moderation
    {
        internal const string Read = "moderation:read";
    }

    internal static class Moderator
    {
        internal const string ManageAutomod = "moderator:manage:automod";
        internal const string ManageAnnouncements = "moderator:manage:announcements";
        internal const string ManageBannedUsers = "moderator:manage:banned_users";
        internal const string ManageChatMessages = "moderator:manage:chat_messages";
        internal const string ReadChatters = "moderator:read:chatters";
        internal const string ReadFollowers = "moderator:read:followers";
        internal const string ReadSuspiciousUsers = "moderator:read:suspicious_users";
    }

    internal static class User
    {
        internal const string ReadEmotes = "user:read:emotes";
        internal const string ReadChat = "user:read:chat";
        internal const string WriteChat = "user:write:chat";
    }
}
