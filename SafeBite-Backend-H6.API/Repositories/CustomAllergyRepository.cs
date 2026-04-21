namespace SafeBite_Backend_H6.API.Repositories;

public class CustomAllergyRepository : BaseRepository<CustomAllergy>, ICustomAllergyRepository
{
    public CustomAllergyRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<CustomAllergy> QueryFilter(string userId, string? searchTerm)
    {
        var query = _context.CustomAllergies.AsNoTracking().Where(a => a.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.Name.Contains(searchTerm.Trim()));
        }

        return query.OrderBy(a => a.Name);
    }

    public async Task<CustomAllergy?> GetAllergyByNameAsync(string name)
    {
        return await _context.CustomAllergies
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public async Task<bool> DeleteAsync(Guid customAllergyId, string userId)
    {
        var entity = await _context.CustomAllergies
            .SingleOrDefaultAsync(x => x.Id == customAllergyId && x.UserId == userId);

        if (entity is null)
            return false;

        _context.CustomAllergies.Remove(entity);
        return true;
    }

    public async Task<CustomAllergy?> GetByIdAsync(Guid customAllergyId, string userId)
    {
        return await _context.CustomAllergies
            .SingleOrDefaultAsync(x => x.Id == customAllergyId && x.UserId == userId);
    }
}
