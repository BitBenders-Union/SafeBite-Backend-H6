using Microsoft.AspNetCore.Mvc;
using SafeBite_Backend_H6.API.Contracts.Requests.OCR;
using SafeBite_Backend_H6.API.Interfaces.Services.OCR;

namespace SafeBiteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OcrController : ControllerBase
    {
        private readonly IOcrService _ocrService;
        private const string DefaultOcrLanguages = "eng+dan+swe+nor+fra";

        public OcrController(IOcrService ocrService)
        {
            _ocrService = ocrService;
        }

        [HttpPost("extract")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Extract([FromForm] OcrExtractRequest request)
        {
            // Validering via DTO'en
            if (request.Image == null || request.Image.Length == 0)
            {
                return BadRequest("No image file uploaded.");
            }

            // Brug værdien fra request eller fald tilbage til standard
            var languages = string.IsNullOrWhiteSpace(request.Lang)
                ? DefaultOcrLanguages
                : request.Lang;

            try
            {
                using var stream = request.Image.OpenReadStream();
                var result = await _ocrService.ExtractTextFromImageAsync(stream, languages);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
