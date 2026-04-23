namespace SafeBite_Backend_H6.API.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserManagementRepository _userManagementRepository;
    private readonly IUserService _userService;

    public UserManagementService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserManagementRepository userManagementRepository, IUserService userService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userManagementRepository = userManagementRepository;
        _userService = userService;
    }

    public Task ActivateUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task AssignRoleAsync(string userId, string roleName)
    {
        throw new NotImplementedException();
    }

    public Task DeactivateUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<IdentityRole>> GetAllRolesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResult<UserResponse>> GetUsersPagedAsync(PaginationParameters parameters, string? searchTerm = null)
    {
        var query = _userManagementRepository.QueryFilter(searchTerm);
        PagedResult<ApplicationUser> result = await _userManagementRepository.GetPagedAsync(parameters, query);

        return result.Map(UserMapping.ToResponse);
    }

    public Task LockUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRoleAsync(string userId, string roleName)
    {
        throw new NotImplementedException();
    }

    public Task UnlockUserAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
