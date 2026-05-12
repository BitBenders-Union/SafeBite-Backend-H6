namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IAllergyUserService
{
    Task<AllergyUserResponse> AddAllergyUserAsync(AllergyUserRequest allergyUserRq, string userId);
    Task<bool> DeleteAllergyUserAsync(Guid id, string userId);
    Task<PagedResult<AllergyUserResponse>> GetAllergyUserPaged(string userId, PaginationParameters parameters, string? searchTerm = null);
    Task<List<AllergyAnalysisItem>> GetAnalysisItemsByUserIdAsync(string userId);
    Task<int> GetTotalCountAsync();
}
