using Discord;
using Discord.Commands;
using Deybot.Music;

namespace Deybot
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMusicService _musicService;
        private static readonly string? _localTestFile = Environment.GetEnvironmentVariable("localTestFile");

        public MusicModule(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Join the voice channel")]
        public async Task JoinAsync(IVoiceChannel? channel = null)
        {
            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("User not in voice channel"); return;
            }

            var audioClient = await channel.ConnectAsync();

            _musicService.ConnectAudioClient(audioClient);
        }

        [Command("playlocal", RunMode = RunMode.Async)]
        [Summary("Play local song")]
        public async Task PlayLocalAsync(IVoiceChannel? channel = null)
        {
            if (_musicService.AudioClient?.ConnectionState != ConnectionState.Connected)
            {
                await JoinAsync(channel);
            }

            if (_localTestFile != null)
            {
                await _musicService.StreamLocalFileAsync(_localTestFile);
            }
        }

        [Command("play")]
        [Summary("Play a YouTube link")]
        public async Task PlayYoutubeAsync([Remainder][Summary("YouTube Link")] string link)
        {
            // TODO
        }

        [Command("leave")]
        [Summary("Leave the voice channel")]
        public async Task LeaveAsync()
        {
            await _musicService.DisconnectAsync();
        }

        [Command("stop")]
        [Summary("Stop playback")]
        public async Task StopPlayingAsync()
        {
            await _musicService.StopPlayingAsync();
        }
    }
}