using GymDB.API.Exceptions;
using System.Net;

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
            catch (HttpException e)
            {
                SetResponse(context, e.StatusCode, e.Message);
            }
            catch (Exception e)
            {
                // TODO - Add Logger

                Console.WriteLine($"!!! Exception: {e.Message} !!!");

                SetResponse(context, HttpStatusCode.InternalServerError, "Something went wrong! Please try again later!");
            }
        }

        private void SetResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.OnStarting((state) =>
            {
                var context2 = (HttpContext)state;

                context2.Response.ContentType = "application/json";
                context2.Response.StatusCode = (int)statusCode;

                context2.Response.WriteAsync(message);

                return Task.CompletedTask;
            }, context);
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
