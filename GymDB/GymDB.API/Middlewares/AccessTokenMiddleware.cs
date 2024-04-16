using GymDB.API.Attributes;
using GymDB.API.Data;
using GymDB.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

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
                Error(context, HttpStatusCode.Unauthorized, "Invalid or missing access token!");
                return;
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
                    Error(context, HttpStatusCode.Unauthorized, "Invalid or empty 'userId' or 'role' claims in access token!");
                    return;
                }

                if (await userService.IsUserWithIdExistAsync(userId))
                {
                    Error(context, HttpStatusCode.Unauthorized, "The current user doesn't exists!");
                    return;
                }

                if (endpointAuth.Roles != null && !endpointAuth.Roles.Contains(roleValue!))
                {
                    Error(context, HttpStatusCode.Forbidden, "You are not authorized to access this endpoint!");
                    return;
                }

                context.User = claimsPrincipal;

                await next(context);  // Call the next delegate/middleware in the pipeline.
            }
            catch
            {
                Error(context, HttpStatusCode.Unauthorized, "Invalid access token!");
                return;
            }
        }

        private CustomAuthorizeAttribute? GetEndpointAuthorization(HttpContext context)
        {
            var currEnpoint = context.GetEndpoint();

            var authorizeAttribute = currEnpoint?.Metadata?.GetMetadata<CustomAuthorizeAttribute>();

            return authorizeAttribute;
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

    public static class AccessTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseAccessTokens(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AccessTokenMiddleware>();
        }
    }
}
