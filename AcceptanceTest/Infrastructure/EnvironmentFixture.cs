using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using WebAPI;
using WebAPI.Factories;

namespace AcceptanceTest.Infrastructure
{
    public class EnvironmentFixture : IDisposable
    {
        public const string RestUrl = "http://127.0.0.1:1337";
        private readonly IHost _host;

        public EnvironmentFixture()
        {
            _host = BuildWebHost();
            _host.RunAsync();
        }

        private IHost BuildWebHost()
        {
            Log.Logger = new LoggerFactory().Create();

            return CreateHostBuilder()
                .UseSerilog()
                .Build();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseStartup<Startup>()
                    .UseUrls(RestUrl));
        
        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}