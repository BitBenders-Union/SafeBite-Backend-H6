namespace SafeBite_Backend_H6.API.Controllers;

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
        return Ok(await _service.GetAllergyUserPaged(parameters, searchterm));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAllergyUser([FromBody] AllergyUserRequest request)
    {
        try
        {
            var response = await _service.AddAllergyUserAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteAllergyUser(Guid Id)
    {
        try
        {
            await _service.DeleteAllergyUserAsync(Id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
}
