using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using GymDB.API.Attributes;
using GymDB.API.Data;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;

namespace GymDB.API.Middlewares
{
    public class AccessTokenMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ApplicationSettings settings;
        private readonly TokenValidationParameters validParams;

        public AccessTokenMiddleware(RequestDelegate next, IConfiguration config)
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

                if (await userService.IsUserWithIdExistAsync(userId))
                {
                    throw new UnauthorizedException("The current user doesn't exists!");
                }

                if (endpointAuth.Roles != null && !endpointAuth.Roles.Contains(roleValue!))
                {
                    throw new ForbiddenException("You are not authorized to access this endpoint!");
                }

                context.User = claimsPrincipal;

                await next(context);  // Call the next delegate/middleware in the pipeline.
            }
            catch (HttpException)
            {
                throw;
            }
            catch
            {
                throw new UnauthorizedException("Invalid access token!");
            }
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
