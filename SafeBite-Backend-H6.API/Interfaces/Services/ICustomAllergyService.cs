namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface ICustomAllergyService
{
    Task<CustomAllergyResponse> AddCustomAllergyAsync(CustomAllergyRequest customAllergyRequest, string userId);
    Task<bool> DeleteAllergyAsync(Guid customAllergyId, string userId);
    Task<PagedResult<CustomAllergyResponse>> GetCustomAllergiesPagedAsync(string userId, PaginationParameters parameters, string? searchTerm = null);
    Task UpdateCustomAllergyAsync(CustomAllergyUpdateRequest customAllergyUpdateRequest, string userId);
    Task<List<AllergyAnalysisItem>> GetAnalysisItemsByUserIdAsync(string userId);
}
