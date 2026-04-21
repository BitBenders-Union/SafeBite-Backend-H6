namespace SafeBite_Backend_H6.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AllergyUserController : ControllerBase
{
    private readonly IAllergyUserService _service;

    public AllergyUserController(IAllergyUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllergyUserPaged([FromQuery] PaginationParameters parameters, [FromQuery] string? searchterm = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.GetAllergyUserPaged(userId, parameters, searchterm);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAllergyUser([FromBody] AllergyUserRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var response = await _service.AddAllergyUserAsync(request, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpDelete("{allergyId}")]
    public async Task<IActionResult> DeleteAllergyUser(Guid allergyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _service.DeleteAllergyUserAsync(allergyId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}
