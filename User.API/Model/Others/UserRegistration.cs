using System.ComponentModel.DataAnnotations;

namespace User.API.Model.Others
{
    public class UserRegistration
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
