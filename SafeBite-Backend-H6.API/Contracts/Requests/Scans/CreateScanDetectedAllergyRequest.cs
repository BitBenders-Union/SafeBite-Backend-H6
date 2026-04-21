namespace SafeBite_Backend_H6.API.Contracts.Requests.Scans;

public class CreateScanDetectedAllergyRequest
{
    public Guid AllergyId { get; set; }
    public List<string> MatchedIngredients { get; set; } = [];
}
