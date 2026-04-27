namespace SafeBite_Backend_H6.API.Interfaces.Repositories;

public interface IUserManagementRepository
{
    IQueryable<ApplicationUser> QueryFilter(string? searchTerm);
    Task<PagedResult<ApplicationUser>> GetPagedAsync(PaginationParameters parameters, IQueryable<ApplicationUser>? query = null);

}
