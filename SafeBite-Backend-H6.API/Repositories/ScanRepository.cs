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
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.CustomAllergy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {

            var normalized = StringHelpers.NormalizeName(searchTerm);

            query = query.Where(s =>
                  s.DetectedAllergies.Any(da =>
                      (da.Allergy != null &&
                       da.Allergy.NormalizedName.Contains(normalized)) ||

                      (da.CustomAllergy != null &&
                       da.CustomAllergy.NormalizedName.Contains(normalized))
                  ) ||
                  s.NormalizedScannedIngredientsText.Contains(normalized)
              );
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
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.CustomAllergy)
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
            .Include(s => s.DetectedAllergies)
                .ThenInclude(sda => sda.CustomAllergy)
            .FirstOrDefaultAsync(s => s.Id == scanId && s.UserId == userId);
    }

    public async Task<Scan?> GetByIdAsync(Guid scanId, string userId)
    {
        return await _context.Scans
            .FirstOrDefaultAsync(s => s.Id == scanId && s.UserId == userId);
    }
}
