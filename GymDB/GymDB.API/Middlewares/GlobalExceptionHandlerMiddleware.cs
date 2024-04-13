using GymDB.API.Exceptions;


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
                context.Response.OnStarting((state) =>
                {
                    var context2 = (HttpContext)state;

                    context2.Response.ContentType = "application/json";
                    context2.Response.StatusCode = e.StatusCode;

                    context2.Response.WriteAsync(e.Message);

                    return Task.CompletedTask;
                }, context);
            }
            catch (Exception e)
            {
                // TODO - Add Logger
                Console.WriteLine($"!!! Exception: {e.Message} !!!");
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
