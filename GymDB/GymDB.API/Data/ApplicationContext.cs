using Microsoft.EntityFrameworkCore;
using GymDB.API.Data.Entities;

namespace GymDB.API.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
