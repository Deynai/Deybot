using Discord.Commands;

namespace Deybot
{
    [Group("sw")]
    public class StopwatchModule : ModuleBase<SocketCommandContext>
    {
        private readonly static Dictionary<string, DateTime> _sw_starts = new();
        private readonly static string _defaultId = "swDefault-zKPGklyEdNo";

        [Command("start")]
        [Summary("Start a stopwatch")]
        public async Task StartStopwatchNamedAsync([Remainder][Summary("Id of the Stopwatch")] string id)
        {
            await Task.Run(async () =>
            {
                if (_sw_starts.ContainsKey(id))
                {
                    await PrintStopwatchAlreadyExistsAsync(Context, id);
                }

                _sw_starts.Add(id, DateTime.Now);
                await PrintStopwatchStarted(Context, id);
            });
        }

        [Command("start")]
        [Summary("Start a stopwatch")]
        public async Task StartStopwatchAsync()
        {
            await StartStopwatchNamedAsync(_defaultId);
        }

        [Command("stop")]
        [Summary("Start a stopwatch")]
        public async Task StopStopwatchNamedAsync([Remainder][Summary("Id of the Stopwatch")] string id)
        {
            await Task.Run(async () =>
            {
                if (_sw_starts.TryGetValue(id, out DateTime startTime))
                {
                    TimeSpan swDuration = DateTime.Now - startTime;
                    _sw_starts.Remove(id);
                    await PrintStopwatchEnded(Context, id, swDuration);
                }
            });
        }
        
        [Command("stop")]
        [Summary("Start a stopwatch")]
        public async Task StopStopwatchAsync()
        {
            await StopStopwatchNamedAsync(_defaultId);
        }

        private static string GetIdToPrint(string id)
        {
            return id == _defaultId ? " " : $" \"{id}\" ";
        }

        private async Task PrintStopwatchStarted(SocketCommandContext context, string id)
        {
            await context.Channel.SendMessageAsync($"Stopwatch{GetIdToPrint(id)}started.");
        }

        private async Task PrintStopwatchEnded(SocketCommandContext context, string id, TimeSpan timeDifference)
        {
            await context.Channel.SendMessageAsync($"Stopwatch{GetIdToPrint(id)}ended. Duration: {(int) timeDifference.TotalMilliseconds}ms");
        }

        private async Task PrintStopwatchAlreadyExistsAsync(SocketCommandContext context, string id)
        {
            await context.Channel.SendMessageAsync($"Stopwatch{GetIdToPrint(id)}already exists");
        }
    }
}
