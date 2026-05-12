using SafeBite_Backend_H6.API.Interfaces.Entities;

namespace SafeBite_Backend_H6.API.Entities.Allergies;

public class CustomAllergy : INormalizedName
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
}
