using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Deybot.Music;

namespace Deybot
{
    internal static class Config
    {
        public static IServiceProvider GetServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<IMusicService, FFmpegService>()
                .BuildServiceProvider();
        }

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