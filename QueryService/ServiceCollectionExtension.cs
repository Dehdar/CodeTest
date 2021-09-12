using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace QueryService
{
    public static class ServiceCollectionExtension
    {
         public static IServiceCollection AddQueryService(this IServiceCollection services) => 
             services.AddMediatR(typeof(ServiceCollectionExtension));
    }
}