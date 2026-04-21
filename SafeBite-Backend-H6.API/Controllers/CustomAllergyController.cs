

namespace SafeBite_Backend_H6.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomAllergyController : ControllerBase
{
    private readonly ICustomAllergyService _customAllergyService;
    
    public CustomAllergyController(ICustomAllergyService customAllergyService)
    {
        _customAllergyService = customAllergyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomAllergiesAsync([FromQuery] PaginationParameters parameters, [FromQuery] string? searchTerm = null)
    {
        var result = await _customAllergyService.GetCustomAllergiesPagedAsync(parameters, searchTerm);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomAllergyAsync([FromBody] CustomAllergyRequest request)
    {
        try
        {
            var response = await _customAllergyService.AddCustomAllergyAsync(request);
            return Ok(response);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCustomAllergyAsync([FromBody] CustomAllergyUpdateRequest request)
    {
        try
        {
            await _customAllergyService.UpdateCustomAllergyAsync(request);
            return Ok();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch( ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteAllergiesAsync(Guid Id)
    {
        try
        {
            bool deleted = await _customAllergyService.DeleteAllergyAsync(Id);
            if (!deleted)
                return NotFound();
            return Ok();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
