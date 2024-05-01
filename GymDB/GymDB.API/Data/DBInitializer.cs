using GymDB.API.Services;

namespace GymDB.API.Data
{
    public static class DBInitializer
    {
        public static async Task<WebApplication> SeedDBAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();

            try
            {
                await context.Database.EnsureCreatedAsync();

                await roleService.EnsureRolesCreatedAsync();

                await roleService.EnsureRootAdminCreatedAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return app;
        }
    }
}
