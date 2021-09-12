using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService
{
    public static class ServiceCollectionExtension
    {
         public static IServiceCollection AddCommandService(this IServiceCollection services) => 
             services
                 .AddTransient<IRandomDelay, RandomDelay>()
                 .AddTransient<ISortingAlgorithm, SortingAlgorithm>()
                 .AddMediatR(typeof(ServiceCollectionExtension));
    }
}