# StreamSharp Panel

StreamSharp Panel is a work-in-progress, cross-platform solution for Twitch streamers to monitor and log their chat, receive feed notifications, and perform moderation actions with ease.
It's written entirely in .NET and Blazor and makes use of [MudBlazors](https://mudblazor.com)'s amazing components.

## Key features
- Real-time chat monitoring with rich message rendering (emotes, mentions, badges)
- Send messages directly within the chat panel
- Full logging of chat messages to file
- View and edit the current stream's information
- Support for moderation actions: ban or timeout users, delete chat messages, review automodded messages
- Notifications for the most important channel events: follows, subscriptions, raids, cheers, redemptions

## Quickstart
1. Build the project using the .NET 10 SDK, or download the latest release
2. The app runs on http://localhost:3000 by default. The listening port can be changed in appsettings.json, but then you'll also have to add a custom client id by registering the application in the [Twitch Developer Console](https://dev.twitch.tv/console/apps) and tweaking the OAuth redirect settings. 
You are advised to do this if you are forking the project.
3. Login to your Twitch account via the in-app settings.
4. Connect and you should start receiving messages and event notifications.

## Currently not available / Coming soon
- Manage Announcements
- Rendering Channel Cheermotes and Channel Badges
- Emote panel and rendering emotes in the message input field
- Making Clips and Raids
- Viewing chatters' informations
- More channel events handling

## Contributing
This is mainly a hobby project but I'd be happy if other people found it useful, 
so please feel free to open issues for bugs or feature requests, or to submit pull requests for improvements!
