using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace Deybot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: null
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
                    services: null
                    );
            }
        }
    }
}