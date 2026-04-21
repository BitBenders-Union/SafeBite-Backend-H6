namespace SafeBite_Backend_H6.API.Contracts.Requests.Allergies;

public class CustomAllergyUpdateRequest
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
