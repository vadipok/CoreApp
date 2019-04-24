using kern.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace PostgresApp
{
    public class ApplicationContext : DbContext
    {
        public DbSet<BaseField> BaseFields { get; set; }

        public DbSet<AccauntUser> AccauntUsers { get; set; }

        public DbSet<AccauntRole> AccauntRoles { get; set; }

        public DbSet<AccauntUsersRole> AccauntUsersRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=kern;Username=postgres;Password=1");
        }
    }
}