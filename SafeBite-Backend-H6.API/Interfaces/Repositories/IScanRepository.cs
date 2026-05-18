namespace SafeBite_Backend_H6.API.Interfaces.Repositories;

public interface IScanRepository : IBaseRepository<Scan>
{
    IQueryable<Scan> QueryByUserId(string userId, string? searchTerm = null, bool? hasDetectedAllergies = null);
    Task<Scan?> GetFullScanByIdAsync(Guid scanId);
    Task<Scan?> GetFullScanByIdAsync(Guid scanId, string userId);
    Task<Scan?> GetByIdAsync(Guid scanId, string userId);
    Task<Scan?> GetScanWithCustomAllergyByUserAndCustomAllergyIdAsync(string userId, Guid customAllergyId);
}
