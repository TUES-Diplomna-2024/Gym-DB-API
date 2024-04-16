using GymDB.API.Data;
/*using GymDB.API.Services.Interfaces;
using GymDB.API.Services;*/
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.PostgreSql;
using GymDB.API.Middlewares;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Repositories;
using GymDB.API.Services.Interfaces;
using GymDB.API.Services;

namespace GymDB.API
{
    public class Program
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            var settings = new ApplicationSettings(builder.Configuration);

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

            builder.Services.AddAuthentication();

            builder.Services.AddAuthorization();

            builder.Services.AddHangfire(options => 
                             options.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(settings.ConnectionStrings.PostgresConnection)));
            
            builder.Services.AddHangfireServer();

            // DB Context
            builder.Services.AddDbContext<ApplicationContext>(c => c.UseNpgsql(settings.ConnectionStrings.PostgresConnection));

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();

            // Custom services
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
        }

        public static async Task ConfigureApplicationAsync(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHangfireDashboard();
            }

            app.UseCors();
            app.UseHttpsRedirection();

            app.UseGlobalExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseAccessTokens();
            app.UseRefreshTokens();

            await app.SeedDBAsync();
        }

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);

            var app = builder.Build();
            await ConfigureApplicationAsync(app);

            app.Run();
        }
    }
}