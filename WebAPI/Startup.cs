using CommandService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using QueryService;
using Repositories;
using Serilog;
using WebAPI.Middleware;

namespace WebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransient(_ => Log.Logger)
                .AddRepositories()
                .AddCommandService()
                .AddQueryService()
                .AddSwaggerGen(c =>
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Number Sorting Algorithm", Version = "vNext" }))
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));
        }

        public void Configure(IApplicationBuilder app)
        {
            app
                .UseMiddleware<CorrelationId>()
                .UseSerilogRequestLogging()
                .UseSwagger()
                .UseSwaggerUI(c =>
                    c.SwaggerEndpoint("v1/swagger.json", "My API V1"))
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}