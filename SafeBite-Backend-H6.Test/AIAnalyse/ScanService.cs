using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using SafeBite_Backend_H6.API.Services.Scans;
using SafeBite_Backend_H6.API.Repositories;
using SafeBite_Backend_H6.API.Interfaces.Services.OCR;
using SafeBite_Backend_H6.API.Shared;
using SafeBite_Backend_H6.API.Interfaces.Repositories;
using SafeBite_Backend_H6.API.Interfaces.Services;
using SafeBite_Backend_H6.API.Contracts.Requests.Scans;
using SafeBite_Backend_H6.API.Contracts.Responses.OCR;
using SafeBite_Backend_H6.API.Contracts.Requests.Analysis;
using SafeBite_Backend_H6.API.Contracts.Responses.Analysis;
using SafeBite_Backend_H6.API.Entities.Scans;

namespace SafeBite_Backend_H6.Test.Services;

public class ScanServiceTests
{
    private readonly Mock<IScanRepository> _repoMock = new();
    private readonly Mock<IOcrService> _ocrMock = new();
    private readonly Mock<IUserAllergyAnalysisService> _userAllergyMock = new();
    private readonly Mock<IScanAnalysisService> _aiMock = new();
    private readonly Mock<IAllergyMatcher> _matcherMock = new();

    private readonly ScanService _sut;

    public ScanServiceTests()
    {
        _sut = new ScanService(
            _repoMock.Object,
            _ocrMock.Object,
            _userAllergyMock.Object,
            _aiMock.Object,
            _matcherMock.Object);
    }

    // Hjælpe-metode til at lave en falsk fil
    private IFormFile CreateFakeImage()
    {
        var content = "fake image content";
        var fileName = "test.png";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        return new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenUserIdIsEmpty()
    {
        // Arrange
        var request = new CreateScanRequest { Image = CreateFakeImage(), Name = "Test Scan" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync("", request));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenOcrFindsNoText()
    {
        // Arrange
        var request = new CreateScanRequest { Image = CreateFakeImage(), Name = "Test" };

        // Jeg simulerer at OCR servicen returnerer en tom streng
        _ocrMock.Setup(test => test.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new OcrResponseDto { IngredientsText = "" });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync("user-123", request));
    }

    [Fact]
    public async Task CreateAsync_ShouldFollowFullFlow_WhenValidRequest()
    {
        // Arrange
        var userId = "user-123";
        var request = new CreateScanRequest { Image = CreateFakeImage(), Name = "Min Scan", Lang = "da" };
        var fakeId = Guid.NewGuid();

        // 1. Mock OCR fund
        _ocrMock.Setup(ocrtest => ocrtest.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(new OcrResponseDto { IngredientsText = "mælk, sukker" });

        // 2. Mock brugerens allergier
        _userAllergyMock.Setup(allergitest => allergitest.GetAllAllergiesForUserAsync(userId))
                        .ReturnsAsync(new List<AllergyAnalysisItem>());

        // 3. Mock AI result
        var aiResult = new ScanAnalysisResult { DetectedAllergies = new() };
        _aiMock.Setup(aitest => aitest.AnalyzeIngredientsAsync(It.IsAny<ScanAnalysisRequest>()))
               .ReturnsAsync(aiResult);

        // 4. Mock Database retur-objekt efter gem
        var savedScan = new Scan { Id = fakeId, Name = "Min Scan", UserId = userId };

        _repoMock.Setup(repo => repo.GetFullScanByIdAsync(It.IsAny<Guid>()))
                 .ReturnsAsync(savedScan);

        // Act
        var result = await _sut.CreateAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Min Scan", result.Name);

        // Tjek om Repository blev kaldt for at gemme
        _repoMock.Verify(add => add.AddAsync(It.IsAny<Scan>()), Times.Once);
        _repoMock.Verify(save => save.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenScanDoesNotExist()
    {
        // Arrange
        var scanId = Guid.NewGuid();
        _repoMock.Setup(scan => scan.GetByIdAsync(scanId, "user1"))
                 .ReturnsAsync((Scan?)null);

        // Act
        var result = await _sut.DeleteAsync("user1", scanId);

        // Assert
        Assert.False(result);
    }
}
