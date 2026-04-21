namespace SafeBite_Backend_H6.API.Mappings;

public class CustomAllergyMappings
{

    public static CustomAllergyResponse ToResponse(CustomAllergy item)
    {
        string allergyName = item.Name.Trim().ToLower();
        return new CustomAllergyResponse
        {
            Id = item.Id,
            Name = allergyName.Length > 0 ? $"{char.ToUpper(allergyName[0])}{allergyName[1..]}" : allergyName
        };
    }

    public static CustomAllergy ToEntity(CustomAllergyRequest request)
    {
        return new CustomAllergy
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Name = request.Name.Trim()
        };
    }

    public static void ToEntityFromUpdateRequest(CustomAllergyUpdateRequest request, CustomAllergy existingCustomAllergy)
    {
        string allergyName = request.Name.Trim().ToLower();
        existingCustomAllergy.Name = allergyName.Length > 0 ? $"{char.ToUpper(allergyName[0])}{allergyName[1..]}" : allergyName;
    }
}
