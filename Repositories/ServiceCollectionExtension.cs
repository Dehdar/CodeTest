using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Infrastructure;

namespace Repositories
{
    public static class ServiceCollectionExtension
    {
         public static IServiceCollection AddRepositories(this IServiceCollection services) => 
             services
                 .AddDbContext<InMemoryDatabase>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("CodeTest"))
                 .AddScoped<InMemoryDatabase>()
                 .AddSingleton<ISortJobRepository, SortJobRepository>()
                 .AddTransient<IDateTimeOffset, DateTimeOffsetWrapper>();  
    }
}