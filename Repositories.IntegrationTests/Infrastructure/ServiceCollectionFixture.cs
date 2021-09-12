using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Repositories.IntegrationTests.Infrastructure
{
    public class ServiceCollectionFixture : IDisposable
    {
        public Mock<IDateTimeOffset> DateTimeOffsetMock { get;  }
        public T GetService<T>() => Provider.GetService<T>();
        
        private  ServiceProvider Provider { get; }

        public ServiceCollectionFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRepositories();

            DateTimeOffsetMock = new Mock<IDateTimeOffset>();
            serviceCollection.AddTransient(_ => DateTimeOffsetMock.Object);

            Provider = serviceCollection.BuildServiceProvider();
        }

        public void Dispose() => Provider?.Dispose();
    }
}