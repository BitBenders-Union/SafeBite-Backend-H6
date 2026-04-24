

namespace SafeBite_Backend_H6.API.Utilities;

public static class StringHelpers
{
    public static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return name.Trim().ToUpperInvariant();
    }

    public static string ToTitleCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var lower = input.Trim().ToLowerInvariant();
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lower);
    }
}
