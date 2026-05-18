namespace SafeBite_Backend_H6.API.Mappings;

public static class CustomAllergyMappings
{

    public static CustomAllergyResponse ToResponse(CustomAllergy item)
    {
        return new CustomAllergyResponse
        {
            Id = item.Id,
            Name = item.Name
        };
    }

    public static CustomAllergy ToEntity(CustomAllergyRequest request, string userId)
    {
        return new CustomAllergy
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = StringHelpers.ToTitleCase(request.Name), 
            NormalizedName = StringHelpers.NormalizeName(request.Name)
        };
    }

    public static void ToEntityFromUpdateRequest(CustomAllergyUpdateRequest request, CustomAllergy existingCustomAllergy)
    {
        existingCustomAllergy.Name = StringHelpers.ToTitleCase(request.Name);
        existingCustomAllergy.NormalizedName = StringHelpers.NormalizeName(request.Name);
    }

    public static void ToEntityForUserUpdate(CustomAllergy existingCustomAllergy, string? userId)
    {
        existingCustomAllergy.UserId = userId;
    }
}
