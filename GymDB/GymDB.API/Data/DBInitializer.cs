using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings;

namespace GymDB.API.Data
{
    public static class DBInitializer
    {
        public static WebApplication SeedDB(this WebApplication app, ApplicationSettings settings)
        {
            using (var scope = app.Services.CreateScope())
            {
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                try
                {
                    context.Database.EnsureCreated();

                    if (!context.Roles.Any())
                    {
                        var roles = settings.DBSeed.Roles.Select(pair => new Role { Id = Guid.NewGuid(),
                                                                                    Name = pair.Key,
                                                                                    NormalizedName = pair.Key.ToUpper().Replace(" ", "_"),
                                                                                    Color = pair.Value })
                                                         .ToList();

                        context.Roles.AddRange(roles);

                        Role superAdminRole = roles.First(role => role.NormalizedName == "SUPER_ADMIN");

                        context.Users.Add(
                            new User {
                                Id = Guid.NewGuid(),
                                Username = settings.DBSeed.RootAdmin.Username,
                                Email = settings.DBSeed.RootAdmin.Email,
                                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(settings.DBSeed.RootAdmin.Password, 13),
                                RoleId = superAdminRole.Id,
                                Role = superAdminRole,
                                BirthDate = DateOnly.FromDateTime(DateTime.UtcNow),
                                Gender = "other",
                                Height = 60,
                                Weight = 60,
                                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                                OnModified = DateTime.UtcNow
                            }
                        );

                        context.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                return app;
            }
        }
    }
}
