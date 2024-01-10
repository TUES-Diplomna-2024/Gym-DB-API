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
                                                                                    NormalizedName = pair.Value.NormalizedName,
                                                                                    Color = pair.Value.Color }
                                                        ).ToList();

                        context.Roles.AddRange(roles);

                        Role rootUserRole = roles.First(role => role.NormalizedName == settings.DBSeed.RootUser.Role);

                        context.Users.Add(
                            new User {
                                Id = Guid.NewGuid(),
                                Username = settings.DBSeed.RootUser.Username,
                                Email = settings.DBSeed.RootUser.Email,
                                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(settings.DBSeed.RootUser.Password, 13),
                                RoleId = rootUserRole.Id,
                                Role = rootUserRole,
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
