namespace SafeBite_Backend_H6.API.Contracts.Responses.Allergies;

public class AllergyResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Icon { get; set; }
}
