using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Model;

namespace User.API.Data.Seed
{
    public class CategoryPreferenceConfiguration : IEntityTypeConfiguration<CategoryPreference>
    {
        public void Configure(EntityTypeBuilder<CategoryPreference> builder)
        {
            builder.HasData
            (
                new CategoryPreference
                {
                    ID = 1,
                    CategoryId = 1,
                    UserId = new Guid("e310a6cb-6677-4aa6-93c7-2763956f7a97").ToString()
                },
                new CategoryPreference
                {
                    ID = 2,
                    CategoryId = 1,
                    UserId = new Guid("398d10fe-4b8d-4606-8e9c-bd2c78d4e001").ToString()
                },
                new CategoryPreference
                {
                    ID = 3,
                    CategoryId = 2,
                    UserId = new Guid("e310a6cb-6677-4aa6-93c7-2763956f7a97").ToString()
                },
                new CategoryPreference
                {
                    ID= 4,
                    CategoryId = 2,
                    UserId = new Guid("398d10fe-4b8d-4606-8e9c-bd2c78d4e001").ToString()
                }
            );
        }
    }
}
