namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface ICustomAllergyService
{
    Task<CustomAllergyResponse> AddCustomAllergyAsync(CustomAllergyRequest customAllergyRequest);
    Task<bool> DeleteAllergyAsync(Guid id);
    Task<PagedResult<CustomAllergyResponse>> GetCustomAllergiesPagedAsync(PaginationParameters parameters, string? searchTerm = null);
    Task UpdateCustomAllergyAsync(CustomAllergyUpdateRequest customAllergyUpdateRequest);
}
