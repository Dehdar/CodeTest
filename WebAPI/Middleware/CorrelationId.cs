using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace WebAPI.Middleware
{
    public class CorrelationId
    {
        private readonly RequestDelegate _next;

        public CorrelationId(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
            {
                await _next(context);
            }
        }
    }
}