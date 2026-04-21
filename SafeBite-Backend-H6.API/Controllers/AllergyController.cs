

namespace SafeBite_Backend_H6.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AllergyController : ControllerBase
{
    private readonly IAllergyService _service;

    public AllergyController(IAllergyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllergiesAsync([FromQuery] PaginationParameters parameters, [FromQuery] string? searchTerm = null)
    {
        return Ok(await _service.GetAllergiesPagedAsync(parameters, searchTerm));
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetAllergyByName(string name)
    {
        try
        {
            AllergyResponse? response = await _service.GetAllergyByNameAsync(name);
            if (response is null)
                return NotFound();
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // scalar viser kun 200 Ok medmindre vi sætter hvad endpointet producere af statuscodes.
    public async Task<IActionResult> AddAllergyAsync([FromBody] AllergyRequest allergyRequest)
    {
        try
        {
            AllergyResponse response = await _service.AddAllergyAsync(allergyRequest);
            return CreatedAtAction(nameof(GetAllergyByName), new { name = response.Name }, response);
            // createdAtAction tager et andet route endpoint, altså skal det være en controller metode,
            // new {name} er parameteren på det valgte endpoint,
            // response er det data vi viser i som resultat
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

    [HttpPut()]
    public async Task<IActionResult> UpdateAllergyAsync(AllergyUpdateRequest AllergyRq)
    {
        try
        {
            await _service.UpdateAllergy(AllergyRq);
            return NoContent();
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAllergyAsync(Guid id)
    {
        try
        {
            bool deleted = await _service.DeleteAllergyAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
