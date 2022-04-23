using System.Diagnostics;

namespace Deybot.Music
{
    public class FFmpeg
    {
        public Process? Process { get; private set; }

        private static readonly string _ffmpegFileName = "ffmpeg";
        private static readonly string _directFileArgs = $"-hide_banner -loglevel quiet -ac 2 -f s16le -ar 48000 pipe:1";

        public FFmpeg()
        {
        }

        public void LoadLocalFile(string path)
        {
            Console.WriteLine($"Loading local path: {path}");
            Process = CreateStream(path, _directFileArgs);
        }

        public void LoadPipeStream()
        {
            Process = CreatePipeStream();
        }

        private Process? CreatePipeStream()
        {
            string args = "-i pipe: -hide_banner -loglevel quiet -ac 2 -f s16le -ar 48000 pipe:1";

            return Process.Start(new ProcessStartInfo
            {
                FileName = _ffmpegFileName,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            });
        }

        private Process? CreateStream(string path, string FFmpegArgs)
        {
            string pathAppendedArgs = FFmpegArgs + $" -i \"{path}\"";

            return Process.Start(new ProcessStartInfo
            {
                FileName = _ffmpegFileName,
                Arguments = pathAppendedArgs,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            });
        }

        public void Dispose()
        {
            Process?.Dispose();
        }
    }
}