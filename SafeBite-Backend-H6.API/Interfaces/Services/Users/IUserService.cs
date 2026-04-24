namespace SafeBite_Backend_H6.API.Interfaces.Services.Users;

public interface IUserService
{
    Task<int> GetTotalActiveUserCount();
    Task<int> GetTotalInactiveUserCount();
    Task<int> GetTotalUserCount();
    Task<PagedResult<UserResponse>> GetUsersPagedAsync(PaginationParameters parameters, string? searchTerm = null);
    Task ActivateUserAsync(string userId);
    Task DeactivateUserAsync(string userId);
    Task<List<RoleResponse>> GetAllRolesAsync();
    Task AssignRoleAsync(string userId, string roleName);
    Task RemoveRoleAsync(string userId, string roleName);
}
