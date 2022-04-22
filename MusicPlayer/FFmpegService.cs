using Discord.Audio;

namespace Deybot.Music
{
    public class FFmpegService : IMusicService
    {
        public IAudioClient? AudioClient { get; private set; }

        private CancellationTokenSource? _cts;

        private Stream? _output;
        private FFmpeg? _ffmpeg;

        public FFmpegService()
        {
        }

        public void ConnectAudioClient(IAudioClient audioClient)
        {
            AudioClient = audioClient;
        }

        public async Task StreamLocalFileAsync(string path)
        {
            if (AudioClient == null)
            {
                Console.WriteLine($"Not connected to audio client");
                return;
            }

            _ffmpeg = new FFmpeg(path);
            _output = _ffmpeg.Process?.StandardOutput.BaseStream;

            if (_output == null)
            {
                Console.WriteLine($"null stream from music service");
                return;
            }

            AudioOutStream discord = AudioClient.CreatePCMStream(AudioApplication.Music);
            _cts = new CancellationTokenSource();

            try
            {
                Console.WriteLine($"Starting to play");
                await _output.CopyToAsync(discord, 96000, _cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await discord.FlushAsync();
            }

            Console.WriteLine($"Finished playing");
            discord.Dispose();
        }

        public async Task StopPlayingAsync()
        {
            _cts?.Cancel();
        }

        public async Task DisconnectAsync()
        {
            _output?.Dispose();
            _ffmpeg?.Dispose();

            if (AudioClient != null)
            {
                Console.WriteLine($"Stopping AudioClient");
                await AudioClient.StopAsync();
            }
        }

        ~FFmpegService()
        {
            _cts?.Dispose();
            _output?.Dispose();
            _ffmpeg?.Dispose();
        }
    }
}