namespace SafeBite_Backend_H6.API.Contracts.Requests.Allergies;

public class AllergyUpdateRequest
{
    public string Id { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }

}
