using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace User.Data
{
    public class UserContextSeed
    {
        public void Seed(UserContext context)
        {
            context.Database.EnsureCreated();

            if (context.Identities.Any())
                return;

            List<User.Model.Identity> identities = new List<User.Model.Identity>();
            identities = PopulateMockIdentities();
            context.Identities.AddRange(identities);
            context.SaveChanges();

            context.Roles.AddRange(PopulateMockRoles());
            context.SaveChanges();

            context.Users.AddRange(PopulateMockUsers(identities));
            context.SaveChanges();

            context.CategoryPreferences.AddRange(PopulateMockCategoryPreferences());
            context.SaveChanges();
        }

        private List<User.Model.Identity> PopulateMockIdentities()
        {
            return new List<User.Model.Identity>()
            {
                new User.Model.Identity(){ ID = Guid.NewGuid(), UserId = 1, UserName = "imGazza", Password = "test" },
                new User.Model.Identity(){ ID = Guid.NewGuid(), UserId = 2, UserName = "clienteUno", Password = "test"}
            };
        }        
        private List<User.Model.Role> PopulateMockRoles()
        {
            return new List<User.Model.Role>()
            {
                new User.Model.Role(){ID = 1, Description = "Admin"},
                new User.Model.Role(){ID = 2, Description = "Customer"}
            };
        }

        private List<User.Model.User> PopulateMockUsers(List<User.Model.Identity> identities)
        {
            return new List<User.Model.User>()
            {
                new User.Model.User(){ FullName = "Luca Gazzardi", Email = "gazza.gazza@gmail.com", IdentityId = identities[0].ID, RoleId = 1 },
                new User.Model.User(){ FullName = "Cliente Casuale", Email = "casual.customer@gmail.com" , IdentityId = identities[1].ID, RoleId = 2}
            };
        }       

        private List<User.Model.CategoryPreference> PopulateMockCategoryPreferences()
        {
            return new List<User.Model.CategoryPreference>()
            {
                new User.Model.CategoryPreference(){ CategoryId = 1, UserId = 1 },
                new User.Model.CategoryPreference(){ CategoryId = 2, UserId = 2 },
                new User.Model.CategoryPreference(){ CategoryId = 3, UserId = 1 },
                new User.Model.CategoryPreference(){ CategoryId = 4, UserId = 2 }
            };
        }
    }
}