using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using GymDB.API.Attributes;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;
using GymDB.API.Data.Settings;

namespace GymDB.API.Middlewares
{
    public class AccessTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly JwtSettings jwtSettings;
        private readonly TokenValidationParameters validParams;

        public AccessTokenMiddleware(RequestDelegate next, IOptions<JwtSettings> settings)
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
                ValidateLifetime = true
            };
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var endpointAuth = GetEndpointAuthorization(context);

            if (endpointAuth == null)
            {
                await next(context);
                return;
            }

            string? auth = context.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer "))
            {
                throw new UnauthorizedException("Invalid or missing access token!");
            }

            string token = auth.Substring("Bearer ".Length).Trim();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(token, validParams, out _);

                string? userIdValue = claimsPrincipal.FindFirst("userId")?.Value;
                string? roleValue = claimsPrincipal.FindFirst("userRole")?.Value;

                Guid userId;

                if (!Guid.TryParse(userIdValue, out userId) || roleValue.IsNullOrEmpty())
                {
                    throw new UnauthorizedException("Invalid or empty 'userId' or 'role' claims in access token!");
                }

                if (!await userService.IsUserWithIdExistAsync(userId))
                {
                    throw new UnauthorizedException("The current user doesn't exists!");
                }

                if (endpointAuth.Roles != null && !endpointAuth.Roles.Contains(roleValue!))
                {
                    throw new ForbiddenException("You are not authorized to access this endpoint!");
                }

                context.User = claimsPrincipal;
            }
            catch (HttpException)
            {
                throw;
            }
            catch
            {
                throw new UnauthorizedException("Invalid access token!");
            }

            // Call the next delegate/middleware in the pipeline.
            await next(context);
        }

        private CustomAuthorizeAttribute? GetEndpointAuthorization(HttpContext context)
        {
            var currEnpoint = context.GetEndpoint();

            var authorizeAttribute = currEnpoint?.Metadata?.GetMetadata<CustomAuthorizeAttribute>();

            return authorizeAttribute;
        }
    }

    public static class AccessTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseAccessTokens(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AccessTokenMiddleware>();
        }
    }
}
