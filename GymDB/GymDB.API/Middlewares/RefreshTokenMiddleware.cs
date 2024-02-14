using GymDB.API.Attributes;
using GymDB.API.Data;
using GymDB.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace GymDB.API.Middlewares
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ApplicationSettings settings;
        private readonly TokenValidationParameters validParams;

        public RefreshTokenMiddleware(RequestDelegate next, IConfiguration config)
        {
            this.next = next;
            settings = new ApplicationSettings(config);

            validParams = new TokenValidationParameters
            {
                ValidIssuer = settings.JwtSettings.Issuer,
                ValidAudience = settings.JwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSettings.ServerSecretKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false
            };
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService, IJwtService jwtService)
        {
            Console.WriteLine("RefreshToken Middleware");

            string? refreshToken = context.Request.Headers["X-Refresh-Token"];
            bool isRefreshTokenRequired = IsRefreshTokenRequiredForEndpoint(context);

            if (!isRefreshTokenRequired && string.IsNullOrEmpty(refreshToken))
            {
                await next(context);
                return;
            } else if (isRefreshTokenRequired && string.IsNullOrEmpty(refreshToken))
            {
                Error(context, HttpStatusCode.Unauthorized, "A refresh token is required!");
                return;
            }

            Console.WriteLine($"REFRESH TOKEN: {refreshToken}");

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(refreshToken, validParams, out _);

                string? userIdValue = claimsPrincipal.FindFirst("userId")?.Value;
                string? expValue = claimsPrincipal.FindFirst("exp")?.Value;

                Guid userId;
                long unixExp;

                if (!Guid.TryParse(userIdValue, out userId) || !long.TryParse(expValue, out unixExp))
                {
                    Error(context, HttpStatusCode.Unauthorized, "Invalid or empty 'userId' or 'exp' claims in refresh token!");
                    return;
                }

                if (userService.GetUserById(userId) == null)
                {
                    Error(context, HttpStatusCode.Unauthorized, "The current user doesn't exists!");
                    return;
                }

                // If Authorization is found in the request headers, it has passed the AccessTokenMiddleware, which has validated it.
                // This means that context.User contains a valid 'userId' claim.

                if (!string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
                {
                    Guid accessTokenUserId;
                    Guid.TryParse(context.User.FindFirst("userId")!.Value, out accessTokenUserId);

                    if (userId != accessTokenUserId)
                    {
                        Error(context, HttpStatusCode.Unauthorized, "The 'userId' claim in the access and refresh tokens do not match!");
                        return;
                    }
                }

                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixExp);
                DateTime expDateTime = dateTimeOffset.UtcDateTime;

                if (isRefreshTokenRequired && DateTime.UtcNow > expDateTime)
                {
                    Error(context, HttpStatusCode.Unauthorized, "Refresh token is provided, but is expired!");
                    return;
                }

                string responseRefreshToken = DateTime.UtcNow > expDateTime ? jwtService.GenerateNewRefreshToken(userId) : refreshToken!;

                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("X-Refresh-Token", responseRefreshToken);

                    return Task.CompletedTask;
                });

                context.User = claimsPrincipal;

                await next(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Error(context, HttpStatusCode.Unauthorized, "Invalid refresh token!");
            }
        }

        private bool IsRefreshTokenRequiredForEndpoint(HttpContext context)
        {
            var currEnpoint = context.GetEndpoint();

            var refreshAttribute = currEnpoint?.Metadata?.GetMetadata<RefreshTokenRequiredAttribute>();

            return refreshAttribute != null;
        }

        private void Error(HttpContext context, HttpStatusCode statusCode, string errorMessage)
        {
            context.Response.OnStarting((state) =>
            {
                var context2 = (HttpContext)state;

                context2.Response.ContentType = "application/json";
                context2.Response.StatusCode = (int)statusCode;

                context2.Response.WriteAsync(errorMessage);

                return Task.CompletedTask;
            }, context);
        }
    }

    public static class RefreshTokenMiddlewareExtentions
    {
        public static IApplicationBuilder UseRefreshToken(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RefreshTokenMiddleware>();
        }
    }
}
