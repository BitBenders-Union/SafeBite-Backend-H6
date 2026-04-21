namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IAllergyUserService
{
    Task<AllergyUserResponse> AddAllergyUserAsync(AllergyUserRequest allergyUserRq);
    Task<bool> DeleteAllergyUserAsync(Guid id);
    Task<PagedResult<AllergyUserResponse>> GetAllergyUserPaged(PaginationParameters parameters, string? searchTerm = null);
}
