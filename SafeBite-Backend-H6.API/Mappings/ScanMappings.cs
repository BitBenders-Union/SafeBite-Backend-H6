namespace SafeBite_Backend_H6.API.Mappings;

public class ScanMappings
{
    public static Scan ToEntity(string userId, string? name, ScanAnalysisResult analysisResult, string rawIngredientsText)
    {
        return new Scan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
            ScannedAt = DateTime.UtcNow,
            ScannedIngredientsText = rawIngredientsText,
            DetectedAllergies = analysisResult.DetectedAllergies
                .Select(ToEntity)
                .ToList()
        };
    }

    public static ScanDetectedAllergies ToEntity(DetectedAllergyAnalysisResult item)
    {
        return new ScanDetectedAllergies
        {
            Id = Guid.NewGuid(),
            AllergyId = item.AllergyId,
            MatchedIngredients = item.MatchedIngredients
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(ToEntity)
                .ToList()
        };
    }

    public static DetectedIngredientMatch ToEntity(string ingredientText)
    {
        return new DetectedIngredientMatch
        {
            Id = Guid.NewGuid(),
            IngredientText = ingredientText.Trim()
        };
    }

    public static ScanResponse ToResponse(Scan scan)
    {
        return new ScanResponse
        {
            Id = scan.Id,
            UserId = scan.UserId,
            Name = scan.Name,
            ScannedAt = scan.ScannedAt,
            ScannedIngredientsText = scan.ScannedIngredientsText,
            DetectedAllergies = scan.DetectedAllergies
                .Select(ToResponse)
                .ToList()
        };
    }

    public static ScanDetectedAllergyResponse ToResponse(ScanDetectedAllergies item)
    {
        return new ScanDetectedAllergyResponse
        {
            Id = item.Id,
            AllergyId = item.AllergyId,
            AllergyName = item.Allergy?.Name ?? string.Empty,
            MatchedIngredients = item.MatchedIngredients
                .Select(ToResponse)
                .ToList()
        };
    }

    public static DetectedIngredientMatchResponse ToResponse(DetectedIngredientMatch item)
    {
        return new DetectedIngredientMatchResponse
        {
            Id = item.Id,
            IngredientText = item.IngredientText
        };
    }
}
