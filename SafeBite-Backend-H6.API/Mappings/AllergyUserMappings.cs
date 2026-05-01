namespace SafeBite_Backend_H6.API.Mappings;

public static class AllergyUserMappings
{
    public static AllergyUserResponse ToResponse(AllergyUser item)
    {
        string allergyName = item.Allergy.Name.Trim().ToLower();
        return new AllergyUserResponse
        {
            UserId = item.UserId,
            AllergyId = item.AllergyId,
            AllergyName = allergyName.Length > 0 ? $"{char.ToUpper(allergyName[0])}{allergyName[1..]}" : allergyName,
            Icon = item.Allergy.Icon
        };
    }

    public static AllergyUser ToEntity(AllergyUserRequest allergyUserRequest, Allergy allergy, string userId)
    {
        return new AllergyUser
        {
            UserId = userId,
            AllergyId = allergyUserRequest.AllergyId,
            Allergy = allergy
        };
    }
}

