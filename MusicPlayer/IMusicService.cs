using Discord.Audio;

namespace Deybot.Music
{
    public interface IMusicService
    {
        public IAudioClient? AudioClient { get; }
        public void ConnectAudioClient(IAudioClient audioClient);
        public Task StreamLocalFileAsync(string path);
        public Task StopPlayingAsync();
        public Task DisconnectAsync();
    }
}
