using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Deybot.Music
{
    public class YTStream
    {
        public Process? Process { get; set; }

        private static readonly string _ytdlFileName = "yt-dlp";
        private static readonly string _ytDomainPrefix = "https://www.youtube.com/watch?v=";
        private static readonly string _ytArgs = "-f bestaudio/best -o -";

        public YTStream() 
        { 
        }

        public async Task LoadYTLink(string id)
        {
            if (Regex.IsMatch(id, @"^[^:/?&=.]*$")) // anything that doesn't contain :, /, ?, =, .
            {
                Process = CreateStream(id);
            }
            else
            {
                // attempt to extract a valid id
                var strippedID = Regex.Match(id, @"^.*?v=([^&]*)"); // match anything up to and including ?v=, then capture anything until "&"
                if (strippedID.Success)
                {
                    Process = CreateStream(strippedID.Groups[1].Value); // groups[0] is the whole match, groups[1] is the first capture group
                }
            }
        }

        private Process? CreateStream(string id)
        {
            string appendedArgs = $"\"{_ytDomainPrefix}{id}\" {_ytArgs}";

            Console.WriteLine($"Cmd Arguments: {appendedArgs}");

            return Process.Start(new ProcessStartInfo
            {
                FileName = _ytdlFileName,
                Arguments = appendedArgs,
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