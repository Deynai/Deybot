using Discord.Commands;

namespace Deybot
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        [Summary("Join the voice channel")]
        public async Task JoinAsync()
        {
            // Join voice channel of user that requested it
        }

        [Command("play")]
        [Summary("Play a YouTube link")]
        public async Task PlayYoutubeAsync([Remainder][Summary("YouTube Link")] string link)
        {
            // Play
        }
    }
}