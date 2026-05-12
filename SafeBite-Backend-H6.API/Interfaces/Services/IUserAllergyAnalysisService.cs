namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IUserAllergyAnalysisService
{
    Task<List<AllergyAnalysisItem>> GetAllAllergiesForUserAsync(string userId);
}
