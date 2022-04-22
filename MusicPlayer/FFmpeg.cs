using System.Diagnostics;

namespace Deybot.Music
{
    public class FFmpeg
    {
        public Process? Process { get; private set; }

        private static readonly string _ffmpegFileName = "ffmpeg";
        private static readonly string _directFileArgs = $"-hide_banner -loglevel quiet -ac 2 -f s16le -ar 48000 pipe:1";

        public FFmpeg(string path)
        {
            Process = CreateStream(path, _directFileArgs);
        }

        public void Dispose()
        {
            Process?.Kill();
            Process?.Dispose();
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
    }
}