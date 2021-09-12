using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CommandService.UnitTests
{
    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void Ensure_Expected_Dependencies_Are_Registered()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddCommandService();
            var provider = collection.BuildServiceProvider();
            
            // Assert
            Assert.NotNull(provider.GetRequiredService<IMediator>());
            Assert.NotNull(provider.GetRequiredService<IRandomDelay>());
            Assert.NotNull(provider.GetRequiredService<ISortingAlgorithm>());
        }
        
        [Fact]
        public void Ensure_SortingAlgorithm_Is_Transient()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddCommandService();
            var provider = collection.BuildServiceProvider();

            // Act
            var firstAlgorithm = provider.GetRequiredService<ISortingAlgorithm>();
            var secondAlgorithm = provider.GetRequiredService<ISortingAlgorithm>();
            
            // Assert
            firstAlgorithm.Should().NotBeSameAs(secondAlgorithm);
        }
    }
}