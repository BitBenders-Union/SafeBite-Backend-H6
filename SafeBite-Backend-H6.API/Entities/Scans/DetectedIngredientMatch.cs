namespace SafeBite_Backend_H6.API.Entities.Scans;

public class DetectedIngredientMatch
{
    public Guid Id { get; set; }
    public Guid ScanDetectedAllergyId { get; set; }
    public string IngredientText { get; set; } = null!;
    public ScanDetectedAllergies ScanDetectedAllergy { get; set; } = null!;
}
