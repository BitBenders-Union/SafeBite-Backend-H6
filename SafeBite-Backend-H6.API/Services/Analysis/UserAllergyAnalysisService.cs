namespace SafeBite_Backend_H6.API.Services.Analysis;

public class UserAllergyAnalysisService : IUserAllergyAnalysisService
{
    private readonly IAllergyUserService _allergyUserService;
    private readonly ICustomAllergyService _customAllergyService;

    public UserAllergyAnalysisService(
        IAllergyUserService allergyUserService,
        ICustomAllergyService customAllergyService)
    {
        _allergyUserService = allergyUserService;
        _customAllergyService = customAllergyService;
    }

    public async Task<List<AllergyAnalysisItem>> GetAllAllergiesForUserAsync(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.");

        var standardAllergies = await _allergyUserService.GetAnalysisItemsByUserIdAsync(userId);
        var customAllergies = await _customAllergyService.GetAnalysisItemsByUserIdAsync(userId);

        // combines the allergiers from both sources
        var combinedAllergies = standardAllergies
            .Concat(customAllergies);

        // removes invald items
        var valid = combinedAllergies
            .Where(x => !string.IsNullOrWhiteSpace(x.AllergyName));

        // removes duplicates by allergyname
        // trims and compares with OrdinalIgnoreCase
        var distinct = valid
            .DistinctBy(x => x.AllergyName.Trim(), StringComparer.OrdinalIgnoreCase);

        // distinctBy runs through the list, remembers the items. if the item is already in the list (using OrdinalIgnoreCase as comparison) it skips the item.
        // this results in a distinct list.

        // note : if a custom allergy has the same name as an item in the standard list of allergies
        // the standard list item will be prioritized when using distinctby since it is found earlier in the combined list. the custom allergies are appended.

        // return as list
        return distinct.ToList();
    }
}
