using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Deybot
{
    internal static class Config
    {
        public static DiscordSocketConfig GetSocketConfig()
        {
            DiscordSocketConfig config = new DiscordSocketConfig();
            config.LogLevel = LogSeverity.Info;
            config.MessageCacheSize = 50;

            return config;
        }

        public static CommandServiceConfig GetCommandsConfig()
        {
            CommandServiceConfig config = new CommandServiceConfig();
            config.LogLevel = LogSeverity.Info;
            config.CaseSensitiveCommands = false;

            return config;
        }
    }
}