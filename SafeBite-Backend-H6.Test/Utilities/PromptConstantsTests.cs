using SafeBite_Backend_H6.API.Contracts.Requests.Analysis;
using SafeBite_Backend_H6.API.Utilities.Constants;


namespace SafeBite_Backend_H6.Test.Utilities;

public class PromptConstantsTests
{
    [Fact]
    public void BuildScanUserPrompt_ShouldIncludeAllAllergyNames()
    {
        // Arrange
        var ingredients = "Hvedemel, vand, salt";
        var allergies = new List<AllergyAnalysisItem>
        {
            new() { AllergyId = Guid.NewGuid(), AllergyName = "Gluten" },
            new() { AllergyId = Guid.NewGuid(), AllergyName = "Nødder" }
        };

        // Act
        var result = PromptConstants.BuildScanUserPrompt(ingredients, allergies);

        // Assert
        Assert.Contains("Gluten", result);
        Assert.Contains("Nødder", result);
        Assert.Contains(ingredients, result);
    }

    [Fact]
    public void BuildScanUserPrompt_ShouldHandleEmptyAllergyList()
    {
        // Arrange
        var ingredients = "Kun vand";
        var allergies = new List<AllergyAnalysisItem>();

        // Act
        var result = PromptConstants.BuildScanUserPrompt(ingredients, allergies);

        // Assert
        // Vi tjekker delelementerne hver for sig, så vi ikke slås med linjeskift
        Assert.Contains("Ingredients text:", result);
        Assert.Contains("Kun vand", result);
        Assert.Contains("User allergy list:", result);
    }
}
