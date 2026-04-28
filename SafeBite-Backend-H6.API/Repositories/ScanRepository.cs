namespace SafeBite_Backend_H6.API.Repositories;

public class ScanRepository : BaseRepository<Scan>, IScanRepository
{
    public ScanRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Scan> QueryByUserId(string userId, string? searchTerm = null, bool? hasDetectedAllergies = null)
    {
        var query = _context.Scans
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.Allergy)
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.MatchedIngredients)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // we only search for allergies. this can also be changed into searching for the scan name.
            // this is to make the query faster, instead of searching for both name, allergyname, ingredientname.
            var trimmed = searchTerm.Trim();

            query = query.Where(s =>
                s.DetectedAllergies.Any(da => da.Allergy.Name.Contains(trimmed)));
        }

        if (hasDetectedAllergies.HasValue)
        {
            query = hasDetectedAllergies.Value
                ? query.Where(s => s.DetectedAllergies.Any())
                : query.Where(s => !s.DetectedAllergies.Any());
        }

        return query.OrderByDescending(s => s.ScannedAt);
    }

    public async Task<Scan?> GetFullScanByIdAsync(Guid scanId)
    {
        return await _context.Scans
            .AsNoTracking()
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.Allergy)
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.MatchedIngredients)
            .FirstOrDefaultAsync(s => s.Id == scanId);
    }

    public async Task<Scan?> GetFullScanByIdAsync(Guid scanId, string userId)
    {
        return await _context.Scans
            .AsNoTracking()
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.Allergy)
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.MatchedIngredients)
            .FirstOrDefaultAsync(s => s.Id == scanId && s.UserId == userId);
    }

    public async Task<Scan?> GetByIdAsync(Guid scanId, string userId)
    {
        return await _context.Scans
            .FirstOrDefaultAsync(s => s.Id == scanId && s.UserId == userId);
    }
}
