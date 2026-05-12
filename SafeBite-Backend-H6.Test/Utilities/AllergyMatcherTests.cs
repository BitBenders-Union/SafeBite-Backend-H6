using SafeBite_Backend_H6.API.Contracts.Requests.Analysis;
using SafeBite_Backend_H6.API.Contracts.Responses.Analysis;
using SafeBite_Backend_H6.API.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafeBite_Backend_H6.Test.Utilities;

public class AllergyMatcherTests
{
    private readonly AllergyMatcher _sut;

    public AllergyMatcherTests()
    {
        _sut = new AllergyMatcher();
    }

    // Tjekker for 
    [Fact]
    public void MatchLocalAllergies_ShouldDetectExactMatch()
    {
        // Arrange
        var text = "Ingredienser: mælk, sukker, salt.";
        var allergies = new List<AllergyAnalysisItem>
        {
            new() { AllergyId = Guid.NewGuid(), AllergyName = "Mælk" }
        };

        // Act
        var result = _sut.MatchLocalAllergies(text, allergies);

        // Assert
        Assert.Single(result);
        Assert.Equal("Mælk", result[0].AllergyName);
        Assert.Contains("mælk", result[0].MatchedIngredients);
    }

    [Fact]
    public void MatchLocalAllergies_ShouldBeCaseInsensitive()
    {
        // Arrange
        var text = "INDEHOLDER MÆLK";
        var allergies = new List<AllergyAnalysisItem>
    {
        new() { AllergyId = Guid.NewGuid(), AllergyName = "Mælk" }
    };

        // Act
        var result = _sut.MatchLocalAllergies(text, allergies);

        // Assert
        Assert.Single(result);

        // tjek om der findes et element, der INDEHOLDER "MÆLK"
        Assert.Contains(result[0].MatchedIngredients, matchedingredient => matchedingredient.Contains("MÆLK", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CleanOcrNoise_ShouldRemoveSimilarOcrErrors()
    {
        // Arrange
        // "mælk" og "mælk (" er meget ens (diff < 7 tegn), så den korte bør "vinde" 
        var text = "mælk, mælk (";
        var allergies = new List<AllergyAnalysisItem>
        {
            new() { AllergyId = Guid.NewGuid(), AllergyName = "Mælk" }
        };

        // Act
        var result = _sut.MatchLocalAllergies(text, allergies);

        // Assert
        Assert.Single(result[0].MatchedIngredients);
        Assert.Equal("mælk", result[0].MatchedIngredients[0]);
    }

    [Fact]
    public void MatchLocalAllergies_ReturnEmpty_WhenNoMatchFound()
    {
        // Arrange
        var text = "Vand, salt, peber.";
        var allergies = new List<AllergyAnalysisItem>
        {
            new() { AllergyId = Guid.NewGuid(), AllergyName = "Mælk" }
        };

        // Act
        var result = _sut.MatchLocalAllergies(text, allergies);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MergeResults_ShouldCombineAiAndLocalMatchesWithoutDuplicates()
    {
        // Arrange
        var allergyId = Guid.NewGuid();
        var aiResult = new ScanAnalysisResult
        {
            DetectedAllergies = new List<DetectedAllergyAnalysisResult>
            {
                new() { AllergyId = allergyId, AllergyName = "Mælk", MatchedIngredients = new List<string> { "mælk" } }
            }
        };

        var localMatches = new List<DetectedAllergyAnalysisResult>
        {
            new() { AllergyId = allergyId, AllergyName = "Mælk", MatchedIngredients = new List<string> { "mælkspulver" } }
        };

        // Act
        _sut.MergeResults(aiResult, localMatches);

        // Assert
        var mælkResult = aiResult.DetectedAllergies.First(a => a.AllergyId == allergyId);
        Assert.Equal(2, mælkResult.MatchedIngredients.Count);
        Assert.Contains("mælk", mælkResult.MatchedIngredients);
        Assert.Contains("mælkspulver", mælkResult.MatchedIngredients);
    }
}
