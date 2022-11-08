using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Model
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } 

        public List<CategoryPreference> CategoryPreferences { get; set; }

    }
}
