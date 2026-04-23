namespace SafeBite_Backend_H6.API.Repositories;

public class UserManagementRepository : IUserManagementRepository
{
    private readonly AuthDbContext _context;
    public UserManagementRepository(AuthDbContext context)
    {
        _context = context;
    }

    public IQueryable<ApplicationUser> QueryFilter(string? searchTerm)
    {
        var query = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(u => u.Email!.Contains(searchTerm) || u.UserName!.Contains(searchTerm));

        return query.OrderBy(u => u.UserName);

    }
    public async Task<PagedResult<ApplicationUser>> GetPagedAsync(PaginationParameters parameters, IQueryable<ApplicationUser>? query = null)
    {
        var source = query ?? _context.Set<ApplicationUser>()
            .AsNoTracking(); // vi laver ikke ændringer så vi behøver ikke at tracke, det gør det hurtigere og mindre ressourcekrævende

        var totalCount = await source.CountAsync();

        var data = await source
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<ApplicationUser>
        {
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize),
            Data = data
        };
    }
}
