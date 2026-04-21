namespace SafeBite_Backend_H6.API.Repositories;

public class CustomAllergyRepository : BaseRepository<CustomAllergy>, ICustomAllergyRepository
{
    public CustomAllergyRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<CustomAllergy> QueryFilter(string? searchTerm)
    {
        var query = _context.CustomAllergies.AsNoTracking();

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
}
