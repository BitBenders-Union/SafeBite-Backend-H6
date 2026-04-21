namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IAllergyService
{
    Task<PagedResult<AllergyResponse>> GetAllergiesPagedAsync(PaginationParameters parameters, string? searchTerm = null);
    Task<AllergyResponse?> GetAllergyByNameAsync(string name);
    Task<Allergy> GetAllergyByIdAsync(Guid id);
    Task<AllergyResponse> AddAllergyAsync(AllergyRequest allergyRequest);
    Task UpdateAllergy(AllergyUpdateRequest allergyRq);
    Task<bool> DeleteAllergyAsync(Guid id);
}
