namespace SafeBite_Backend_H6.API.Repositories;

public class AllergyRepository : BaseRepository<Allergy>, IAllergyRepository
{
    // context findes i base repository
    public AllergyRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Allergy> QueryFilter(string? searchTerm)
    {
        var query = _context.Allergies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.Name.Contains(searchTerm.Trim()));
        }

        return query.OrderBy(a => a.Name);
    }

    public async Task<Allergy?> GetAllergyByNameAsync(string name)
    {
        return await _context.Allergies
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Name.ToLower().Trim() == name.ToLower());
    }
}

