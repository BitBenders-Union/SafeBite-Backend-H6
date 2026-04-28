namespace SafeBite_Backend_H6.API.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserManagementRepository _userManagementRepository;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserManagementRepository userManagementRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userManagementRepository = userManagementRepository;
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

    public async Task ActivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsDeactivated = false;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!await _roleManager.RoleExistsAsync(roleName))
            throw new KeyNotFoundException("Role does not exist");

        if (await _userManager.IsInRoleAsync(user, roleName))
            return;

        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task DeactivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsDeactivated = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<List<RoleResponse>> GetAllRolesAsync()
    {
        var result = await _roleManager.Roles.ToListAsync();

        if (result == null)
            throw new KeyNotFoundException("No roles found");

        return result.Select(RoleMapping.ToResponse).ToList();
    }

    public async Task<PagedResult<UserResponse>> GetUsersPagedAsync(PaginationParameters parameters, string? searchTerm = null)
    {
        var query = _userManagementRepository.QueryFilter(searchTerm);
        PagedResult<ApplicationUser> result = await _userManagementRepository.GetPagedAsync(parameters, query);

        return result.Map(UserMapping.ToResponse);
    }

    public async Task RemoveRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!await _userManager.IsInRoleAsync(user, roleName))
            return;

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

}
