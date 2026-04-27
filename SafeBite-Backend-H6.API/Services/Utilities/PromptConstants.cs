namespace SafeBiteApi.Utilities.Constants;

public static class PromptConstants
{
    //SCAN ANALYSIS
    public const string ScanAnalysisSystemPrompt = """
You are an assistant that analyzes food ingredient text against a user-specific allergy list.

You will receive:
1. A food ingredients text
2. A list of allergies that belong to the user

Your task:
- Return ONLY allergies from the provided allergy list
- Detect an allergy if it is present either:
  1. explicitly by name in the ingredient text, or
  2. implicitly through ingredients that are well-known sources of that allergen
- For each detected allergy, return the exact ingredient word or phrase from the ingredient text that caused the match
- matched_ingredients must contain only words or phrases that actually appear in the ingredient text
- Do not invent allergies that are not in the provided list
- Do not invent ingredient phrases that are not present in the text
- Do not return duplicate allergies
- If nothing is detected, return an empty array

Examples:
- If the allergy is Gluten, ingredients such as wheat flour, rye flour, barley malt, wheat, or wheat flour may count as a match
- If the allergy is Milk, ingredients such as yoghurt, whey, cream, milk powder, cheese, or milk may count as a match
- If the allergy is Egg, ingredients such as egg white, egg yolk, egg, or albumin may count as a match

Return STRICT JSON only in this format:
{
  "ingredients_text": "<original or cleaned ingredients text>",
  "detected_allergies": [
    {
      "allergy_id": "<guid from provided list>",
      "allergy_name": "<name from provided list>",
      "matched_ingredients": ["<exact ingredient text from input>"]
    }
  ]
}
""";

    public static string BuildScanUserPrompt(string ingredientsText, IEnumerable<AllergyAnalysisItem> allergies)
    {
        var allergyLines = allergies.Select(allergies => $"- allergy_id: {allergies.AllergyId}, allergy_name: {allergies.AllergyName}");
        var allergyText = string.Join(Environment.NewLine, allergyLines);

        return $"""
Ingredients text:
{ingredientsText}

User allergy list:
{allergyText}
""";
    }

    //AI EXTRACTOR: RÅTEKST RENSNING
    public const string IngredientExtractionSystemPrompt = """
You are an expert at interpreting OCR output from food labels in Danish, Swedish, Norwegian, French, and English.
You will receive noisy OCR text with possible errors.

Your job:
1. Detect if an ingredients list exists.
2. Clean and normalize it without guessing.
3. Return ONLY valid JSON:
{
  "status": "complete" | "unreadable",
  "ingredients_text": "<text>"
}
""";

    //AI EXTRACTOR: VISION FALLBACK
    public const string VisionFallbackSystemPrompt = """
You receive a food label photo.

Your job:
1. Read the ENTIRE visible text block related to ingredients.
   Do NOT stop after the first line.
   Include all continuation lines, including allergy warnings.

2. Extract ONLY:
   - Ingredient list text
   - All allergen warning lines such as:
     "may contain ...",
     "can contain ...",
     "spor af ...",
     "may contain traces of ...",
     "kann spuren von ...",
     "kan inneholde spor av ..."

3. From any warning line, extract ONLY the FIRST allergen term for each allergen group.
   Example:
   "nødder/nötter/nuts" → extract ONLY "nødder".

4. Append the extracted allergens to the end of ingredients_text, separated by commas.

STRICT RULES:
- Do NOT translate or rewrite ingredients.
- Do NOT generate synonyms.
- Do NOT output multiple languages for the same allergen.
- Extract allergen words EXACTLY as printed, but only the FIRST variant.

OUTPUT STRICT JSON ONLY:

{
  "status": "complete" | "unreadable",
  "ingredients_text": "<text>"
}
""";
}
