using Microsoft.AspNetCore.Identity;

namespace User.API.Repository
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(User.Model.User user, string password);
        Task AddRoleAsync(User.Model.User user, string role);
        Task<User.Model.User> GetUserByEmailAsync(string email);
        Task<bool> CheckUserPassword(User.Model.User user, string password);
        Task<string> GetSingleRoleAsync(User.Model.User user);

        Task<Dictionary<string, string>> GetEmailsToNotify(int categoryId);


    }
}
