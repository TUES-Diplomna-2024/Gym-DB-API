using GymDB.API.Data;
using GymDB.API.Services.Interfaces;
using GymDB.API.Services;
using Microsoft.EntityFrameworkCore;

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

            // DB Context
            builder.Services.AddDbContext<ApplicationContext>(c => c.UseNpgsql(settings.PostgresConnectionString));

            // Custom services
            builder.Services.AddScoped<IUserService, UserService>();
        }

        public static void ConfigureApplication(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var settings = new ApplicationSettings(builder.Configuration);

            ConfigureServices(builder, settings);

            var app = builder.Build();

            ConfigureApplication(app);

            app.Run();
        }
    }
}