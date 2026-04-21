namespace SafeBite_Backend_H6.API.Repositories;

public class AllergyUserRepository : BaseRepository<AllergyUser>, IAllergyUserRepository
{
    public AllergyUserRepository(AppDbContext context) : base(context)
    {
    }
    public IQueryable<AllergyUser> QueryFilter(string userId, string? searchTerm)
    {
        var query = _context.AllergyUsers
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(au => au.Allergy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.Allergy.Name.Contains(searchTerm.Trim()));
        }

        return query.OrderBy(a => a.Allergy.Name);
    }

    public async Task<bool> DeleteAsync(Guid allergyId, string userId)
    {
        var existingAllergyUser = await _context.AllergyUsers
            .SingleOrDefaultAsync(au => au.AllergyId == allergyId && au.UserId == userId);

        if (existingAllergyUser is null)
            return false;

        _context.AllergyUsers.Remove(existingAllergyUser);
        return true;

    }
}
