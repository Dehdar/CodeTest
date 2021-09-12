using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using WebAPI.Factories;

namespace WebAPI
{
    public static class Program
    {
        public static void Main()
        {
            Log.Logger = new LoggerFactory().Create();
            
            try
            {
                Log.Information($"Starting WebAPI");
                CreateHostBuilder()
                    .UseSerilog()
                    .Build()
                    .Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "WebAPI crashed!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}