namespace SafeBite_Backend_H6.API.Contracts.Responses.Scans;

public class ScanDetectedAllergyResponse
{
    public Guid Id { get; set; }
    public string AllergyName { get; set; } = null!;
    public List<DetectedIngredientMatchResponse> MatchedIngredients { get; set; } = [];
}
