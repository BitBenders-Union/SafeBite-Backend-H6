

using SafeBite_Backend_H6.API.Interfaces.Entities;

namespace SafeBite_Backend_H6.API.Entities.Allergies;

public class Allergy : INormalizedName
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string NormalizedName { get; set; } = null!;
    public string Icon { get; set; }
    public ICollection<AllergyUser> AllergyUser { get; set; } = [];
    public ICollection<ScanDetectedAllergies> ScanDetectedAllergies { get; set; } = [];
}
