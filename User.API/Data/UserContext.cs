using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;
using User.API.Data.Seed;
using User.Model;

namespace User.Data
{
    public class UserContext : IdentityDbContext<Model.User>
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User.Model.User> Users { get; set; }
        public DbSet<User.Model.CategoryPreference> CategoryPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User.Model.User>().ToTable("User");
            modelBuilder.Entity<User.Model.CategoryPreference>().ToTable("CategoryPreference");

            modelBuilder.Entity<CategoryPreference>()
                .HasOne(u => u.User)
                .WithMany(c => c.CategoryPreferences)
                .HasForeignKey(u => u.UserId)
                .HasPrincipalKey(x => x.Id);

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            SeedUsers(modelBuilder);
            modelBuilder.ApplyConfiguration(new CategoryPreferenceConfiguration());
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {

            string adminRoleId = new Guid("780f92ba-bc4a-4603-ae82-a1371a238c11").ToString();
            string customerRoleId = new Guid("9ae35cde-ce65-4bdd-9d05-689bac3ca7fc").ToString();
            PasswordHasher<Model.User> ph = new PasswordHasher<Model.User>();

            var user1 = new Model.User
            {
                Id = new Guid("e310a6cb-6677-4aa6-93c7-2763956f7a97").ToString(),
                FullName = "Luca Gazzardi",
                Email = "gazza@it.it",
                UserName = "gazza@it.it",
                NormalizedUserName = "GAZZA@IT.IT",
                NormalizedEmail = "GAZZA@IT.IT"
            };
            var user2 = new Model.User
            {
                Id = new Guid("398d10fe-4b8d-4606-8e9c-bd2c78d4e001").ToString(),
                FullName = "Percorso Microservizi",
                Email = "per.mic@it.it",
                UserName = "per.mic@it.it",
                NormalizedUserName = "PER.MIC@IT.IT",
                NormalizedEmail = "PER.MIC@IT.IT"
            };

            user1.PasswordHash = ph.HashPassword(user1, "Pass@word");
            user2.PasswordHash = ph.HashPassword(user2, "Pass@word");

            modelBuilder.Entity<Model.User>().HasData
            (
                user1, user2
            );

            #region SeedRoles

            modelBuilder.Entity<IdentityRole>().HasData
            (
                new IdentityRole
                {
                    Id = customerRoleId,                    
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData
            (
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = user1.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = customerRoleId,
                    UserId = user2.Id
                }
            );

            #endregion
        }
    }
}