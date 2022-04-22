using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Deybot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private IServiceProvider _serviceProvider;

        public CommandHandler(IServiceProvider serviceProvider, DiscordSocketClient client, CommandService commands)
        {
            _serviceProvider = serviceProvider;
            _client = client;
            _commands = commands;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _serviceProvider
                );
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage msg) { return; }
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) { return; }

            int pos = 0;
            if (msg.HasCharPrefix(']', ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
            {
                SocketCommandContext context = new SocketCommandContext(_client, msg);

                IResult result = await _commands.ExecuteAsync(
                    context: context, 
                    argPos: pos, 
                    services: _serviceProvider
                    );
            }
        }
    }
}