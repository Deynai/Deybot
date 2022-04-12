using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Deybot
{
    public partial class Deybot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private readonly CommandHandler _commandHandler;

        public static Task Main(string[] args)
        {
            return new Deybot().MainAsync(); 
        }

        private Deybot()
        {
            // Set Env variable - included in .gitignore
            Env.SetVars(); 

            _client = new DiscordSocketClient(Config.GetSocketConfig());
            _commands = new CommandService(Config.GetCommandsConfig());

            _commandHandler = new CommandHandler(_client, _commands);

            _client.Log += LogHandler.Log;
            _commands.Log += LogHandler.Log;
        }

        public async Task MainAsync()
        {
            var token = Environment.GetEnvironmentVariable("token");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await _commandHandler.InstallCommandsAsync();

            await Task.Delay(-1);
        }

        ~Deybot()
        {
            _client.Log -= LogHandler.Log;
            _commands.Log -= LogHandler.Log;
        }
    }
}