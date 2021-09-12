using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommandService
{
    public interface ISortingAlgorithm
    {
        Task<IEnumerable<int>> Sort(IEnumerable<int> numbers, CancellationToken cancellationToken);
    }
    
    public class SortingAlgorithm : ISortingAlgorithm
    {
        private readonly IRandomDelay _randomDelay;
    
        public SortingAlgorithm(IRandomDelay randomDelay)
        {
            _randomDelay = randomDelay;
        }

        public async Task<IEnumerable<int>> Sort(IEnumerable<int> numbers, CancellationToken cancellationToken)
        {
            await _randomDelay.DelayAsync(
                @from: TimeSpan.FromMilliseconds(1000),
                to: TimeSpan.FromMilliseconds(10000), cancellationToken);
            
            return numbers?.OrderBy(i => i);
        }
    }
}