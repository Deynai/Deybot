using Discord.Audio;

namespace Deybot.Music
{
    public class FFmpegService : IMusicService
    {
        public IAudioClient? AudioClient { get; private set; }

        private AudioOutStream? _discordAudioStream;
        private bool _currentlyPlaying = false;
        private CancellationTokenSource? _queueCts; // entire queue cancel
        private CancellationTokenSource? _ffcts; // ffmpeg->discord copy cancel
        private CancellationTokenSource? _ytcts; // ytcts->ffmpeg copy cancel

        private readonly Queue<SongRequest> _songQueue;

        public FFmpegService()
        {
            _songQueue = new Queue<SongRequest>();
        }

        public void ConnectAudioClient(IAudioClient audioClient)
        {
            AudioClient = audioClient;
        }

        public async Task RequestSongAsync(Platform platform, string path)
        {
            SongRequest request = new SongRequest
            {
                Platform = platform,
                ID = path,
            };

            Console.WriteLine($"Queueing Song");
            _songQueue.Enqueue(request);

            Console.WriteLine($"Checking if we are currently playing: {_currentlyPlaying}");
            if (_currentlyPlaying) { return; }

            Console.WriteLine($"Not currently playing. Starting Internal Queue");
            await PlayQueue();
        }

        private async Task PlayQueue()
        {
            // Start up internal queue stream
            _currentlyPlaying = true;
            _queueCts = new CancellationTokenSource();

            while (_songQueue.Count > 0)
            {
                SongRequest request = _songQueue.Dequeue();

                switch (request.Platform)
                {
                    case Platform.Local:
                        await LocalInternalAsync(request);
                        break;
                    case Platform.Youtube:
                        await YoutubeInternalAsync(request);
                        break;
                    default:
                        break;
                }

                if (_queueCts.Token.IsCancellationRequested)
                {
                    Console.WriteLine($"Hard Stop requested - clearing queue");
                    _songQueue.Clear();
                }
            }

            // On queue empty dispose of resources
            Console.WriteLine($"Queue empty - closing down stream");
            _currentlyPlaying = false;
        }

        private async Task LocalInternalAsync(SongRequest request)
        {
            string path = request.ID;

            if (AudioClient == null)
            {
                Console.WriteLine($"Not connected to audio client");
                return;
            }

            if (_discordAudioStream == null)
            {
                Console.WriteLine($"Creating DiscordAudioStream");
                _discordAudioStream = AudioClient.CreatePCMStream(AudioApplication.Music);
            }

            Console.WriteLine($"Loading ffmpeg");
            FFmpeg ffmpeg = new FFmpeg();
            ffmpeg.LoadLocalFile(path);

            if (ffmpeg.Process == null)
            {
                Console.WriteLine($"Failed to create ffmpeg stream");
                return;
            }

            Stream output = ffmpeg.Process.StandardOutput.BaseStream;

            if (output == null)
            {
                Console.WriteLine($"null stream from music service");
                return;
            }

            try
            {
                _ffcts = new CancellationTokenSource();
                Console.WriteLine($"Starting to play");
                await output.CopyToAsync(_discordAudioStream, 96000, _ffcts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await _discordAudioStream.FlushAsync();
            }

            Console.WriteLine($"Finished playing track");
            ffmpeg?.Dispose();
            _ffcts?.Dispose();
        }

        private async Task YoutubeInternalAsync(SongRequest request)
        {
            if (AudioClient == null)
            {
                Console.WriteLine($"Not connected to audio client");
                return;
            }

            if (_discordAudioStream == null)
            {
                Console.WriteLine($"Creating DiscordAudioStream");
                _discordAudioStream = AudioClient.CreatePCMStream(AudioApplication.Music);
            }

            Console.WriteLine($"Creating FFmpeg Stream");
            FFmpeg ffmpeg = new FFmpeg();
            ffmpeg.LoadPipeStream();

            if (ffmpeg.Process == null)
            {
                Console.WriteLine($"Failed to create ffmpeg stream");
                return;
            }

            Stream ffmpegInput = ffmpeg.Process.StandardInput.BaseStream;
            Stream ffmpegOutput = ffmpeg.Process.StandardOutput.BaseStream;

            Console.WriteLine($"Creating YTStream");
            YTStream ytStream = new YTStream();
            await ytStream.LoadYTLink(request.ID);

            if (ytStream.Process == null)
            {
                Console.WriteLine($"Failed to create yt stream");
                return;
            }

            Stream ytOutput = ytStream.Process.StandardOutput.BaseStream;

            try
            {
                _ffcts = new CancellationTokenSource();
                _ytcts = new CancellationTokenSource();
                Console.WriteLine($"Starting yt and ff tasks");
                Task ytTask =  ytOutput.CopyToAsync(ffmpegInput, 96000, _ytcts.Token);
                Task ffmpegTask = ffmpegOutput.CopyToAsync(_discordAudioStream, 96000, _ffcts.Token);

                Console.WriteLine($"Tasks started");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                OnFinishedTrack(ffmpeg, ytStream);
                return;
            }

            Console.WriteLine($"Finally, await Flush ytOutput");
            await ytOutput.FlushAsync();
            ytOutput.Dispose();

            Console.WriteLine($"Finally, await Flush ffmpegInput");
            await ffmpegInput.FlushAsync();
            ffmpegInput.Dispose();

            Console.WriteLine($"Finally, await Flush ffmpegOutput");
            await ffmpegOutput.FlushAsync();
            ffmpegOutput.Dispose();

            Console.WriteLine($"Flush discord audio");
            await _discordAudioStream.FlushAsync();

            OnFinishedTrack(ffmpeg, ytStream);
        }

        private void OnFinishedTrack(FFmpeg ffmpeg, YTStream ytStream)
        {
            Console.WriteLine($"Finished playing track");
            ffmpeg?.Dispose();
            ytStream?.Dispose();
            _ffcts?.Dispose();
            _discordAudioStream?.Dispose();
            _discordAudioStream = null;
        }

        public async Task SkipSongAsync()
        {
            CancelCopyTokens();
        }

        public async Task StopPlayingAsync()
        {
            _queueCts?.Cancel();
            CancelCopyTokens();
        }

        public async Task DisconnectAsync()
        {
            await StopPlayingAsync();

            if (AudioClient != null)
            {
                Console.WriteLine($"Stopping AudioClient");
                await AudioClient.StopAsync();
            }
        }

        private void CancelCopyTokens()
        {
            _ffcts?.Cancel();
            _ytcts?.Cancel();
        }

        ~FFmpegService()
        {
            _queueCts?.Cancel();
            CancelCopyTokens();
        }
    }
}