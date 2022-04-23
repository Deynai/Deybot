using Discord.Audio;

namespace Deybot.Music
{
    public interface IMusicService
    {
        public IAudioClient? AudioClient { get; }
        public void ConnectAudioClient(IAudioClient audioClient);
        public Task RequestSongAsync(Platform platform, string path);
        public Task SkipSongAsync();
        public Task StopPlayingAsync();
        public Task DisconnectAsync();
    }
}
