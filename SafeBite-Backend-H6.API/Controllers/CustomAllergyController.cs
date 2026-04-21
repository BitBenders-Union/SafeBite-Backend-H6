namespace SafeBite_Backend_H6.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomAllergyController : ControllerBase
{
    private readonly ICustomAllergyService _customAllergyService;

    public CustomAllergyController(ICustomAllergyService customAllergyService)
    {
        _customAllergyService = customAllergyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomAllergiesAsync(
        [FromQuery] PaginationParameters parameters,
        [FromQuery] string? searchTerm = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _customAllergyService.GetCustomAllergiesPagedAsync(userId, parameters, searchTerm);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomAllergyAsync([FromBody] CustomAllergyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var response = await _customAllergyService.AddCustomAllergyAsync(request, userId);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCustomAllergyAsync([FromBody] CustomAllergyUpdateRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            await _customAllergyService.UpdateCustomAllergyAsync(request, userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{customAllergyId}")]
    public async Task<IActionResult> DeleteAllergyAsync(Guid customAllergyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            await _customAllergyService.DeleteAllergyAsync(customAllergyId, userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
