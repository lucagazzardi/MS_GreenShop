using Microsoft.AspNetCore.Identity;
using System.Text;
using User.API.Model.Others;
using User.Data;
using User.Model;

namespace User.API.Repository
{
    public class UserService : IUserService
    {
        private readonly UserManager<User.Model.User> _userManager;
        private readonly UserContext _userContext;

        public UserService(UserManager<User.Model.User> userManager, UserContext userContext)
        {
            _userManager = userManager;
            _userContext = userContext;
        }

        public async Task<IdentityResult> CreateUserAsync(User.Model.User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddRoleAsync(User.Model.User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<User.Model.User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckUserPassword(User.Model.User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<string> GetSingleRoleAsync(User.Model.User user)
        {
            return (await _userManager.GetRolesAsync(user)).First();
        }

        public async Task<Dictionary<string, string>> GetEmailsToNotify(int categoryId)
        {
            Dictionary<string, string> emails = new Dictionary<string, string>();
            var categoryPreferences = _userContext.CategoryPreferences.Where(x => x.CategoryId == categoryId).ToList();

            foreach(var userId in categoryPreferences.Select(x => x.UserId).ToList())
            {
                var user = await _userManager.FindByIdAsync(userId);
                emails.Add(userId, user.Email);
            }

            return emails;
        }

    }
}
