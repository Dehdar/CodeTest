using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Repositories.IntegrationTests
{
    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void Ensure_Expected_Dependencies_Are_Registered()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddRepositories();
            var provider = collection.BuildServiceProvider();
            
            // Assert
            Assert.NotNull(provider.GetRequiredService<ISortJobRepository>());
        }
        
        [Fact]
        public void Ensure_ISortJobRepository_Is_Singleton()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddRepositories();
            var provider = collection.BuildServiceProvider();

            // Act
            var firstAlgorithm = provider.GetRequiredService<ISortJobRepository>();
            var secondAlgorithm = provider.GetRequiredService<ISortJobRepository>();
            
            // Assert
            firstAlgorithm.Should().BeSameAs(secondAlgorithm);
        }
    }
}