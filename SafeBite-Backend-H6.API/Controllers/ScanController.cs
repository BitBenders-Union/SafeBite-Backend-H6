namespace SafeBite_Backend_H6.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ScanController : ControllerBase
{
    private readonly IScanService _scanService;

    public ScanController(IScanService scanService)
    {
        _scanService = scanService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateScanRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _scanService.CreateAsync(userId, request);

        return CreatedAtAction(nameof(GetById), new { scanId = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParameters parameters, [FromQuery] string? searchTerm = null, [FromQuery] bool? hasDetectedAllergies = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _scanService.GetPagedByUserIdAsync(userId, parameters, searchTerm, hasDetectedAllergies);

        return Ok(result);
    }

    [HttpGet("{scanId}")]
    public async Task<IActionResult> GetById(Guid scanId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _scanService.GetByIdAsync(userId, scanId);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{scanId}")]
    public async Task<IActionResult> Delete(Guid scanId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var deleted = await _scanService.DeleteAsync(userId, scanId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
