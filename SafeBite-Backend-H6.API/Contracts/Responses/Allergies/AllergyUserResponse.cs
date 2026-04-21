namespace SafeBite_Backend_H6.API.Contracts.Responses.Allergies;

public class AllergyUserResponse
{
    public string UserId { get; set; }
    public Guid AllergyId { get; set; }
    public string AllergyName { get; set; } = string.Empty;

}
