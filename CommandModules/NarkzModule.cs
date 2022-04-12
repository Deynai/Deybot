using Discord.Commands;

namespace Deybot
{
    [Group("narkz")]
    public class NarkzModule : ModuleBase<SocketCommandContext>
    {
        [Command("")]
        [Summary("Water is wet")]
        public async Task LuckyAsync()
        {
            await Context.Channel.SendMessageAsync($"https://i.imgur.com/OKXOru4.png");
        }
    }
}
