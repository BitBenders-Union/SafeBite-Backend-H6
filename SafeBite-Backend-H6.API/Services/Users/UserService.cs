

namespace SafeBite_Backend_H6.API.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // should only be accsible for admin users
    public async Task<int> GetTotalUserCount()
    {
        return await _userManager.Users.CountAsync();
    }

    public async Task<int> GetTotalActiveUserCount()
    {
        return await _userManager.Users.Where(u => !u.IsDeactivated).CountAsync();
    }

    public async Task<int> GetTotalInactiveUserCount()
    {
        return await _userManager.Users.Where(u => u.IsDeactivated).CountAsync();
    }

}
