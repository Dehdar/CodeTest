using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.TestDependencies;
using Xunit;

namespace CommandService.UnitTests
{
    public class SortingAlgorithmTests
    {
        [Theory, AutoMoqData]
        public async Task Sort_Numbers_Ascending(
            SortingAlgorithm sut,
            CancellationToken token)
        {
            // Arrange
            var numbers = new List<int> { 1, 2, 3, 9, 7, 5, -1, 100, 0 };
            var expected = new List<int> { -1, 0, 1, 2, 3, 5, 7, 9, 100 };
            
            // Act
            var sortedNumbers = await sut.Sort(numbers, token);

            // Assert
            Assert.True(sortedNumbers.SequenceEqual(expected));
        }
        
        [Theory, AutoMoqData]
        public async Task Return_Null_If_Input_Is_Null(
            SortingAlgorithm sut,
            CancellationToken token)
        {
            // Act
            var sortedNumbers = await sut.Sort(null, token);
            
            // Assert
            Assert.Null(sortedNumbers);
        }
        
        [Theory, AutoMoqData]
        public async Task Return_Empty_Collection_If_Input_Is_Empty(
            SortingAlgorithm sut,
            CancellationToken token)
        {
            // Act
            var sortedNumbers = await sut.Sort(Enumerable.Empty<int>(), token);
            
            // Assert
            Assert.Empty(sortedNumbers);
        }
    }
}