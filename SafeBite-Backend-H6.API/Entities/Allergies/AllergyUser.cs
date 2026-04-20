namespace SafeBite_Backend_H6.API.Entities.Allergies;

public class AllergyUser
{
    public string UserId { get; set; } = null!;
    public Guid AllergyId { get; set; }
    public Allergy Allergy { get; set; } = null!;
}
