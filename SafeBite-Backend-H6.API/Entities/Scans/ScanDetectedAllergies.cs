namespace SafeBite_Backend_H6.API.Entities.Scans;

public class ScanDetectedAllergies
{
    public Guid Id { get; set; }
    public Guid ScanId { get; set; }
    public Guid AllergyId { get; set; }
    public Scan Scan { get; set; } = null!;
    public Allergy Allergy { get; set; } = null!;
    public List<DetectedIngredientMatch> MatchedIngredients { get; set; } = [];
}
