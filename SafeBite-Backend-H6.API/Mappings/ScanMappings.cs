namespace SafeBite_Backend_H6.API.Mappings;

public static class ScanMappings
{
    public static Scan ToEntity(string userId, string? name, ScanAnalysisResult analysisResult, string rawIngredientsText)
    {
        return new Scan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = !string.IsNullOrWhiteSpace(name) ? StringHelpers.ToTitleCase(name) : null,
            ScannedAt = DateTime.UtcNow,
            ScannedIngredientsText = rawIngredientsText,
            DetectedAllergies = analysisResult.DetectedAllergies
                .Select(ToScanDetectedAllergyEntity)
                .ToList()
        };
    }

    public static ScanDetectedAllergies ToScanDetectedAllergyEntity(DetectedAllergyAnalysisResult item)
    {
        return new ScanDetectedAllergies
        {
            Id = Guid.NewGuid(),
            AllergyId = item.AllergyId,
            MatchedIngredients = item.MatchedIngredients
                .Where(mi => !string.IsNullOrWhiteSpace(mi))
                .Select(ToDetectedIngredientMatchEntity)
                .ToList()
        };
    }

    public static DetectedIngredientMatch ToDetectedIngredientMatchEntity(string ingredientText)
    {
        return new DetectedIngredientMatch
        {
            Id = Guid.NewGuid(),
            IngredientText = ingredientText.Trim()
        };
    }

    public static ScanResponse ToScanResponse(Scan scan)
    {
        return new ScanResponse
        {
            Id = scan.Id,
            UserId = scan.UserId,
            Name = scan.Name,
            ScannedAt = scan.ScannedAt,
            ScannedIngredientsText = scan.ScannedIngredientsText,
            DetectedAllergies = scan.DetectedAllergies
                .Select(ToScanDetectedAllergyResponse)
                .ToList()
        };
    }

    public static ScanDetectedAllergyResponse ToScanDetectedAllergyResponse(ScanDetectedAllergies item)
    {
        return new ScanDetectedAllergyResponse
        {
            Id = item.Id,
            AllergyId = item.AllergyId,
            AllergyName = item.Allergy?.Name ?? string.Empty,
            MatchedIngredients = item.MatchedIngredients
                .Select(ToDetectedIngredientMatchResponse)
                .ToList()
        };
    }

    public static DetectedIngredientMatchResponse ToDetectedIngredientMatchResponse(DetectedIngredientMatch item)
    {
        return new DetectedIngredientMatchResponse
        {
            Id = item.Id,
            IngredientText = item.IngredientText
        };
    }
}
