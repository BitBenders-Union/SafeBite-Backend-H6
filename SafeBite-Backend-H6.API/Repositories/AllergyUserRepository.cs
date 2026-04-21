namespace SafeBite_Backend_H6.API.Repositories;

public class AllergyUserRepository : BaseRepository<AllergyUser>, IAllergyUserRepository
{
    public AllergyUserRepository(AppDbContext context) : base(context)
    {
    }
    public IQueryable<AllergyUser> QueryFilter(string? searchTerm)
    {
        var query = _context.AllergyUsers
            .AsNoTracking()
            .Include(au => au.Allergy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.Allergy.Name.Contains(searchTerm.Trim()));
        }

        return query.OrderBy(a => a.Allergy.Name);
    }
}
