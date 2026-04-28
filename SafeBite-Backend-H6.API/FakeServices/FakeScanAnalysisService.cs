namespace SafeBite_Backend_H6.API.FakeServices;

public class FakeScanAnalysisService : IScanAnalysisService
{
    public Task<ScanAnalysisResult> AnalyzeIngredientsAsync(ScanAnalysisRequest request)
    {
        var detectedAllergies = request.Allergies
            .Where(a =>
                StringHelpers.NormalizeName(a.AllergyName) == "PEANUT")
            .Select(a => new DetectedAllergyAnalysisResult
            {
                AllergyId = a.AllergyId,
                AllergyName = a.AllergyName,
                MatchedIngredients = ["Peanut"]
            })
            .ToList();

        var response = new ScanAnalysisResult
        {
            IngredientsText = request.IngredientsText?.Trim() ?? "",
            DetectedAllergies = detectedAllergies
        };

        return Task.FromResult(response);
    }
}
