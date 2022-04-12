using Discord.Commands;

namespace Deybot
{
    [Group("timespam")]
    public class TimeSpamModule : ModuleBase<SocketCommandContext>
    {
        private readonly static Queue<CancellationTokenSource> _cts = new();

        [Command("start")]
        [Summary("Prints the current date and time every interval")]
        public async Task TimerAsync([Remainder][Summary("Timer Interval")] string interval)
        {
            if (int.TryParse(interval, out int seconds))
            {
                CancellationTokenSource source = new();
                CancellationToken token = source.Token;
                _cts.Enqueue(source);

                await Task.Run(() => PrintTimeOnIntervalAsync(Context, seconds, token));
            }
        }

        [Command("stop")]
        [Summary("Stop one timer - FIFO")]
        public async Task StopTimerAsync()
        {
            await Task.Run(() =>
            {
                CancellationTokenSource source = _cts.Dequeue();
                source.Cancel();
            });
        }

        [Command("nuke")]
        [Summary("Stop all timers")]
        public async Task StopAllTimersAsync()
        {
            await Task.Run(() =>
            {
                while (_cts.Count > 0)
                {
                    CancellationTokenSource source = _cts.Dequeue();
                    source.Cancel();
                }
            });
        }

        public async void PrintTimeOnIntervalAsync(SocketCommandContext context, int seconds, CancellationToken ct)
        {
            Console.WriteLine($"Timer started");

            while (!ct.IsCancellationRequested)
            {
                var time = DateTime.Now;
                var msg = await context.Channel.SendMessageAsync($"{DateTime.Now,-19}");
                var delay = DateTime.Now - time;
                var newTimeToWait = (int)((seconds - delay.TotalSeconds) * 1000);
                Console.WriteLine(msg.ToString());

                if (newTimeToWait > 0)
                {
                    try { await Task.Delay(newTimeToWait, ct); }
                    catch { break; }
                }
            }

            Console.WriteLine($"Timer with {seconds} interval stopped");
        }
    }
}
