using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace WebAPI.Factories
{
    public class LoggerFactory
    {
        public Logger Create() =>
            new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate:"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{CorrelationId}] {Message} {NewLine}{Exception}")
                .CreateLogger();
    }
}