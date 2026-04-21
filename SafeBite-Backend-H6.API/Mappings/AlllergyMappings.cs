
namespace SafeBite_Backend_H6.API.Mappings;

public static class AllergyMappings
{
    public static AllergyResponse ToResponse(Allergy item)
    {
        string name = item.Name.Trim().ToLower();
        return new AllergyResponse
        {
            Id = item.Id,
            Name = name.Length > 0 ? $"{char.ToUpper(name[0])}{name[1..]}" : name,
            // check om der er mere end 0 chars. tag det første char og gør det uppercase, tag resten af stringen eller tag stringen
            // x..y er en range operator : https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#range-operator-
            Icon = item.Icon,
        };
    }

    public static Allergy ToEntity(AllergyRequest request)
    {
        return new Allergy
        {
            Name = request.Name.Trim(),
            Icon = request.Icon == null ? string.Empty : request.Icon.Trim(),
        };
    }

    public static void ToEntityFromUpdateRequest(AllergyUpdateRequest request, Allergy existingAllergy)
    {
        string name = request.Name.Trim().ToLower();
        existingAllergy.Name = name.Length > 0 ? $"{char.ToUpper(name[0])}{name[1..]}" : name;
        existingAllergy.Icon = request.Icon == null ? string.Empty : request.Icon.Trim();

    }

}

