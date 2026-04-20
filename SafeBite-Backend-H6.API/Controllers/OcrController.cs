using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Extract([FromForm] IFormFile image, [FromForm] string? lang)
        {
            // Validering af input
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file uploaded.");
            }

            // Bestem sprog (fallback til standard hvis intet er valgt)
            var languages = string.IsNullOrWhiteSpace(lang) ? DefaultOcrLanguages : lang;

            try
            {
                // Åbn stream og kør processen
                using var stream = image.OpenReadStream();
                var result = await _ocrService.ExtractTextFromImageAsync(stream, languages);

                // Returnér resultatet
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log fejlen
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
