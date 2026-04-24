using SafeBiteV2.API.Contracts.Responses.UserManagement;

namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IUserManagementService
{
    Task<PagedResult<UserResponse>> GetUsersPagedAsync(PaginationParameters parameters, string? searchTerm = null);
    Task ActivateUserAsync(string userId);
    Task DeactivateUserAsync(string userId);
    Task<List<RoleResponse>> GetAllRolesAsync();
    Task AssignRoleAsync(string userId, string roleName);
    Task RemoveRoleAsync(string userId, string roleName);
}
