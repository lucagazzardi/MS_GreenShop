using System.ComponentModel.DataAnnotations.Schema;

namespace User.Model
{
    public class User
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public Guid IdentityId { get; set; }

        [ForeignKey("IdentityId")]
        public Identity Identity { get; set; }
        public Role Role { get; set; }
        public List<CategoryPreference> CategoryPreferences { get; set; }
        
    }
}
