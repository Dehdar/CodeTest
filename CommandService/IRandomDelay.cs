using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommandService
{
    public interface IRandomDelay
    {
        Task DelayAsync(TimeSpan from, TimeSpan to, CancellationToken cancellationToken);
    }
    
    public class RandomDelay : IRandomDelay
    {
        public Task DelayAsync(TimeSpan from, TimeSpan to, CancellationToken cancellationToken = new())
        {
            return Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(
                    from.Milliseconds, 
                    to.Milliseconds)), 
                cancellationToken);
        }
    }
}