using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using GymDB.API.Attributes;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;
using GymDB.API.Data.Settings;

namespace GymDB.API.Middlewares
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly JwtSettings jwtSettings;
        private readonly TokenValidationParameters validParams;

        public RefreshTokenMiddleware(RequestDelegate next, IOptions<JwtSettings> settings)
        {
            this.next = next;
            jwtSettings = settings.Value;

            validParams = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey(),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false
            };
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService, IJwtService jwtService)
        {
            string? refreshToken = context.Request.Headers["X-Refresh-Token"];
            bool isRefreshTokenRequired = IsRefreshTokenRequiredForEndpoint(context);

            if (!isRefreshTokenRequired && string.IsNullOrEmpty(refreshToken))
            {
                await next(context);
                return;
            } else if (isRefreshTokenRequired && string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedException("A refresh token is required!");
            }

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
                    throw new UnauthorizedException("Invalid or empty 'userId' or 'exp' claims in refresh token!");
                }

                if (!await userService.IsUserWithIdExistAsync(userId))
                {
                    throw new UnauthorizedException("The current user doesn't exists!");
                }

                // If 'userId' claim of context.User is not null, it has passed the AccessTokenMiddleware, which has validated it.

                if (context.User.FindFirst("userId") != null)
                {
                    Guid accessTokenUserId;
                    Guid.TryParse(context.User.FindFirst("userId")!.Value, out accessTokenUserId);

                    if (userId != accessTokenUserId)
                    {
                        throw new UnauthorizedException("The 'userId' claim in the access and refresh tokens do not match!");
                    }
                }

                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixExp);
                DateTime expDateTime = dateTimeOffset.UtcDateTime;

                if (isRefreshTokenRequired && DateTime.UtcNow > expDateTime)
                {
                    throw new UnauthorizedException("Refresh token is provided, but is expired!");
                }

                string responseRefreshToken = DateTime.UtcNow > expDateTime ? jwtService.GenerateNewRefreshToken(userId) : refreshToken!;

                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("X-Refresh-Token", responseRefreshToken);

                    return Task.CompletedTask;
                });

                context.User = claimsPrincipal;
            }
            catch (HttpException)
            {
                throw;
            }
            catch
            {
                throw new UnauthorizedException("Invalid refresh token!");
            }

            await next(context);
        }

        private bool IsRefreshTokenRequiredForEndpoint(HttpContext context)
        {
            var currEnpoint = context.GetEndpoint();

            var refreshAttribute = currEnpoint?.Metadata?.GetMetadata<RefreshTokenRequiredAttribute>();

            return refreshAttribute != null;
        }
    }

    public static class RefreshTokenMiddlewareExtentions
    {
        public static IApplicationBuilder UseRefreshTokens(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RefreshTokenMiddleware>();
        }
    }
}
