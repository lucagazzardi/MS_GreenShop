using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using User.Model;

namespace User.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User.Model.User> Users { get; set; }
        public DbSet<User.Model.Role> Roles { get; set; }
        public DbSet<User.Model.CategoryPreference> CategoryPreferences { get; set; }
        public DbSet<User.Model.Identity> Identities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User.Model.User>().ToTable("User");
            modelBuilder.Entity<User.Model.Role>().ToTable("Role");
            modelBuilder.Entity<User.Model.CategoryPreference>().ToTable("CategoryPreference");
            modelBuilder.Entity<User.Model.Identity>().ToTable("Identity");

            modelBuilder.Entity<Identity>()
                .HasIndex(u => u.UserName)
                .IsUnique();
        }
    }
}