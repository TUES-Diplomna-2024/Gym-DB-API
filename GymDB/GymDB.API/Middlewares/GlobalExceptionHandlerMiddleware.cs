using Microsoft.AspNetCore.Http;

namespace GymDB.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.OnStarting((state) =>
                {
                    var context2 = (HttpContext)state;

                    context2.Response.ContentType = "application/json";
                    context2.Response.StatusCode = 500;

                    context2.Response.WriteAsync($"ZDR, BEBCE: {ex.Message}");

                    return Task.CompletedTask;
                }, context);
            }
        }
    }

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
