using Microsoft.AspNetCore.Mvc;
using SafeBite_Backend_H6.API.Helpers.Form_Data_Helper;
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
        public async Task<IActionResult> Extract([FromForm] ExtractRequest request)
        {
            // Validering af input
            if (request == null || request.Image.Length == 0)
            {
                return BadRequest("No image file uploaded.");
            }

            // Bestem sprog (fallback til standard hvis intet er valgt)
            var languages = string.IsNullOrWhiteSpace(request.Lang) ? DefaultOcrLanguages : request.Lang;

            try
            {
                // Åbn stream og kør processen
                using var stream = request.Image.OpenReadStream();
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
