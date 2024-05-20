using GymDB.API.Services.Interfaces;

namespace GymDB.API.Data
{
    public static class DbInitializer
    {
        public static async Task<WebApplication> SeedDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

            await context.Database.EnsureCreatedAsync();
            await roleService.EnsureRolesCreatedAsync();
            await roleService.EnsureRootAdminCreatedAsync();

            return app;
        }
    }
}
