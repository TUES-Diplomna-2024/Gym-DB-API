using GymDB.API.Data;
using GymDB.API.Services.Interfaces;
using GymDB.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using Hangfire.PostgreSql;

namespace GymDB.API
{
    public class Program
    {
        public static void ConfigureServices(WebApplicationBuilder builder, ApplicationSettings settings)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("*")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = settings.JwtIssuer,
                        ValidAudience = settings.JwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSecretKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddHangfire(options => 
                             options.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(settings.PostgresConnectionString)));
            
            builder.Services.AddHangfireServer();

            // DB Context
            builder.Services.AddDbContext<ApplicationContext>(c => c.UseNpgsql(settings.PostgresConnectionString));

            // Custom services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
        }

        public static void ConfigureApplication(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHangfireDashboard();
            }

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();
        }

        public static void ConfigureRecurringJobs(ApplicationSettings settings)
        {
            RecurringJob.AddOrUpdate<ISessionService>("remove-inactive-sessions-job", service => service.RemoveAllInactiveSessions(), "0 */2 * * *");
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var settings = new ApplicationSettings(builder.Configuration);

            ConfigureServices(builder, settings);

            var app = builder.Build();

            ConfigureApplication(app);

            ConfigureRecurringJobs(settings);

            app.Run();
        }
    }
}