namespace SafeBite_Backend_H6.API.Entities.Scans;

public class ScanDetectedAllergies
{
    public Guid Id { get; set; }
    public Guid ScanId { get; set; }
    public Scan Scan { get; set; } = null!;
    public Guid? AllergyId { get; set; }
    public Allergy? Allergy { get; set; }
    public Guid? CustomAllergyId { get; set; }
    public CustomAllergy? CustomAllergy { get; set; }
    public List<DetectedIngredientMatch> MatchedIngredients { get; set; } = [];
}
