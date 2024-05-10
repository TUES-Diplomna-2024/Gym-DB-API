using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Hangfire;
using Hangfire.PostgreSql;
using Azure.Storage;
using GymDB.API.Middlewares;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Repositories;
using GymDB.API.Services.Interfaces;
using GymDB.API.Services;
using GymDB.API.Data;
using GymDB.API.Data.Settings;
using GymDB.API.Data.Settings.HelperClasses;

namespace GymDB.API
{
    public class Program
    {
        public static void ConfigureSettings(WebApplicationBuilder builder)
        {
            builder.Services.AddOptions<ConnectionStrings>().BindConfiguration("ConnectionStrings")
                            .ValidateDataAnnotations().ValidateOnStart();

            builder.Services.AddOptions<AzureSettingsConfig>().BindConfiguration("AzureSettings")
                            .ValidateDataAnnotations().ValidateOnStart();

            builder.Services.AddOptions<AzureSettings>()
                            .Configure<IOptions<AzureSettingsConfig>>((settings, config) =>
                            {
                                var azureConfig = config.Value;

                                settings.ImageContainer = azureConfig.ImageContainer;
                                settings.Credential = new StorageSharedKeyCredential(azureConfig.StorageAccount, azureConfig.AccessKey);
                                settings.BaseBlobUri = new Uri($"https://{azureConfig.StorageAccount}.blob.core.windows.net/");
                                settings.AcceptedFileMimeTypes = azureConfig.AcceptedFileMimeTypes.Split(';').ToList();
                                settings.MaxFileSize = azureConfig.MaxFileSize;
                            });

            builder.Services.AddOptions<JwtSettings>().BindConfiguration("JwtSettings")
                            .ValidateDataAnnotations().ValidateOnStart();

            builder.Services.AddOptions<RoleColors>().BindConfiguration("DbSeed:RoleColors")
                            .ValidateDataAnnotations().ValidateOnStart();

            builder.Services.AddOptions<RoleSettings>()
                            .Configure<IOptions<RoleColors>>((settings, config) =>
                            {
                                var roleColors = config.Value;

                                settings.SuperAdmin = new RoleDefinition("Super Admin", "SUPER_ADMIN", roleColors.SuperAdmin);
                                settings.Admin = new RoleDefinition("Admin", "ADMIN", roleColors.Admin);
                                settings.Normie = new RoleDefinition("Normie", "NORMIE", roleColors.Normie);
                            });

            builder.Services.AddOptions<RootAdmin>().BindConfiguration("DbSeed:RootAdmin")
                            .ValidateDataAnnotations().ValidateOnStart();
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
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

            var connectionStrings = builder.Services.BuildServiceProvider()
                                                    .GetService<IOptions<ConnectionStrings>>()!.Value;

            builder.Services.AddHangfire(options =>
                             options.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionStrings.PostgresConnection)));

            builder.Services.AddHangfireServer();

            // Db Context
            builder.Services.AddDbContext<ApplicationContext>(c => c.UseNpgsql(connectionStrings.PostgresConnection));

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
            builder.Services.AddScoped<IAzureBlobRepository, AzureBlobRepository>();
            builder.Services.AddScoped<IExerciseImageRepository, ExerciseImageRepository>();
            builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();

            // Custom services
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IExerciseService, ExerciseService>();
            builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();
            builder.Services.AddScoped<IExerciseImageService, ExerciseImageService>();
            builder.Services.AddScoped<IWorkoutService, WorkoutService>();
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

            app.UseGlobalExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseAccessTokens();
            app.UseRefreshTokens();

            app.SeedDBAsync().GetAwaiter().GetResult();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureSettings(builder);
            ConfigureServices(builder);

            var app = builder.Build();
            
            ConfigureApplication(app);

            app.Run();
        }
    }
}