namespace SafeBite_Backend_H6.API.Mappings;

public static class AllergyMappings
{
    public static AllergyResponse ToResponse(Allergy item)
    {
        return new AllergyResponse
        {
            Id = item.Id,
            Name = item.Name, // formatted at save time
            Icon = item.Icon,
        };
    }

    public static Allergy ToEntity(AllergyRequest request)
    {
        return new Allergy
        {
            Name = StringHelpers.ToTitleCase(request.Name),
            NormalizedName = StringHelpers.NormalizeName(request.Name),
            Icon = request.Icon?.Trim() ?? string.Empty,
        };
    }

    public static void ToEntityFromUpdateRequest(AllergyUpdateRequest request, Allergy existingAllergy)
    {

        existingAllergy.Name = StringHelpers.ToTitleCase(request.Name);
        existingAllergy.NormalizedName = StringHelpers.NormalizeName(request.Name);
        existingAllergy.Icon = request.Icon?.Trim() ?? string.Empty;
    }

}

