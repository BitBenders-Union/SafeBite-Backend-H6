namespace SafeBite_Backend_H6.API.Utilities;

public class AllergyMatcher : IAllergyMatcher
{
    public List<DetectedAllergyAnalysisResult> MatchLocalAllergies(string text, List<AllergyAnalysisItem> allergies)
    {
        var detected = new List<DetectedAllergyAnalysisResult>();

        // Split text baseret på delimiters
        var segments = text.Split(new[] { ',', ';', '.', '\n', ':', '-' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim())
                           .Where(s => !string.IsNullOrWhiteSpace(s))
                           .ToList();

        foreach (var allergy in allergies)
        {
            if (AllergyMappings.Mappings.TryGetValue(allergy.AllergyName, out var keywords))
            {
                var foundIngredients = new List<string>();

                foreach (var segment in segments)
                {
                    if (keywords.Any(keyword => segment.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    {
                        foundIngredients.Add(segment);
                    }
                }

                if (foundIngredients.Any())
                {
                    detected.Add(new DetectedAllergyAnalysisResult
                    {
                        AllergyId = allergy.AllergyId,
                        AllergyName = allergy.AllergyName,
                        MatchedIngredients = CleanOcrNoise(foundIngredients)
                    });
                }
            }
        }
        return detected;
    }

    public void MergeResults(ScanAnalysisResult ai, List<DetectedAllergyAnalysisResult> local)
    {
        foreach (var localItem in local)
        {
            var existingAiItem = ai.DetectedAllergies.FirstOrDefault(a => a.AllergyId == localItem.AllergyId);

            if (existingAiItem == null)
            {
                ai.DetectedAllergies.Add(localItem);
            }
            else
            {
                var combined = existingAiItem.MatchedIngredients
                    .Concat(localItem.MatchedIngredients)
                    .ToList();

                existingAiItem.MatchedIngredients = CleanOcrNoise(combined);
            }
        }
    }

    private List<string> CleanOcrNoise(List<string> input)
    {
        var distinct = input.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var final = new List<string>();

        foreach (var item in distinct)
        {
            if (!final.Any(f => IsTooSimilar(f, item)))
            {
                final.Add(item);
            }
        }
        return final;
    }

    private bool IsTooSimilar(string existing, string candidate)
    {
        if (string.Equals(existing, candidate, StringComparison.OrdinalIgnoreCase)) return true;

        string longer = existing.Length >= candidate.Length ? existing : candidate;
        string shorter = existing.Length < candidate.Length ? existing : candidate;

        if (longer.Contains(shorter, StringComparison.OrdinalIgnoreCase))
        {
            var diffLength = longer.Length - shorter.Length;
            if (diffLength < 7) return true;
        }

        return false;
    }
}
