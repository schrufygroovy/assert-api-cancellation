using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebApi31
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger<ErrorHandlingMiddleware> logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (OperationCanceledException exception)
            {
                this.logger.LogInformation(exception, $"Request was canceled: {context.Request?.QueryString}");
                context.Response.ContentType = "application/json";
                // https://httpstatuses.com/499
                context.Response.StatusCode = 499;
            }
        }
    }
}
