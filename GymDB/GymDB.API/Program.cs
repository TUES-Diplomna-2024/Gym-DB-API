using GymDB.API.Data;
using GymDB.API.Services.Interfaces;
using GymDB.API.Services;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.PostgreSql;
using GymDB.API.Middlewares;

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

            builder.Services.AddAuthentication();

            builder.Services.AddAuthorization();

            builder.Services.AddHangfire(options => 
                             options.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(settings.ConnectionStrings.PostgresConnection)));
            
            builder.Services.AddHangfireServer();

            // DB Context
            builder.Services.AddDbContext<ApplicationContext>(c => c.UseNpgsql(settings.ConnectionStrings.PostgresConnection));

            // Custom services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IExerciseService, ExerciseService>();
            builder.Services.AddScoped<IWorkoutService, WorkoutService>();
            builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();
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

            app.UseAccessToken();

            app.UseRefreshToken();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var settings = new ApplicationSettings(builder.Configuration);

            ConfigureServices(builder, settings);

            var app = builder.Build();

            ConfigureApplication(app);

            app.SeedDB(settings);

            app.Run();
        }
    }
}