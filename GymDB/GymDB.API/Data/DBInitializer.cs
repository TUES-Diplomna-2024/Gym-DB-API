using GymDB.API.Data.Entities;
using GymDB.API.Mapping;
using System.Data;

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
                        context.SaveChanges();
                    }

                    if (!context.Users.Any())
                    {
                        Role? superAdminRole = context.Roles.FirstOrDefault(role => role.NormalizedName == "SUPER_ADMIN");

                        if (superAdminRole == null)
                            throw new Exception("Role with normalized name 'SUPER_ADMIN' could not be found!");

                        context.Users.Add(settings.DBSeed.RootAdmin.ToEntity(superAdminRole));
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
