

namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IScanService
{
    Task<ScanResponse> CreateAsync(string userId, CreateScanRequest request);

    Task<PagedResult<ScanResponse>> GetPagedByUserIdAsync(string userId, PaginationParameters parameters, string? searchTerm = null, bool? hasDetectedAllergies = null);

    Task<ScanResponse?> GetByIdAsync(string userId, Guid scanId);

    Task<bool> DeleteAsync(string userId, Guid scanId);
    Task<int> GetTotalCountAsync();
}
