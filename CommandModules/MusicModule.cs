using Discord;
using Discord.Commands;
using Deybot.Music;

namespace Deybot
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMusicService _musicService;
        private static readonly string? _localMusicFolder = Environment.GetEnvironmentVariable("localMusicFolder");

        public MusicModule(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Join the voice channel")]
        public async Task JoinAsync(IVoiceChannel? channel = null)
        {
            Console.WriteLine($"Called JoinAsync with context: {Context.User.Username}, {(Context.User as IGuildUser)?.VoiceChannel}");

            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("User not in voice channel"); return;
            }

            Console.WriteLine($"Connecting to channel");
            var audioClient = await channel.ConnectAsync();

            Console.WriteLine($"Successfully connected, linking audio client to music service");
            _musicService.ConnectAudioClient(audioClient);
        }

        [Command("playlocal", RunMode = RunMode.Async)]
        [Summary("Play local file")]
        public async Task PlayLocalAsync([Remainder][Summary("Local Filename")] string filename)
        {
            if (_musicService.AudioClient?.ConnectionState != ConnectionState.Connected)
            {
                await JoinAsync();
            }

            Console.WriteLine($"{_localMusicFolder}");
            if (_localMusicFolder != null)
            {
                await _musicService.RequestSongAsync(Platform.Local, _localMusicFolder + filename);
            }
        }

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Play a YouTube link")]
        public async Task PlayYoutubeAsync([Remainder][Summary("YouTube Link")] string id)
        {
            if (_musicService.AudioClient?.ConnectionState != ConnectionState.Connected)
            {
                await JoinAsync();
            }

            if (id != null)
            {
                await _musicService.RequestSongAsync(Platform.Youtube, id);
            }
        }

        [Command("stop")]
        [Summary("Stop playback")]
        public async Task StopPlayingAsync()
        {
            await _musicService.StopPlayingAsync();
        }

        [Command("skip")]
        [Summary("Skip to next song")]
        public async Task SkipAsync()
        {
            await _musicService.SkipSongAsync();
        }

        [Command("leave")]
        [Summary("Leave the voice channel")]
        public async Task LeaveAsync()
        {
            await _musicService.DisconnectAsync();
        }
    }
}