using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SafeBite_Backend_H6.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;
    public HealthCheckController(IHealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealthCheck()
    {
        var response = await _healthCheckService.HealthCheckPass();
        if (response)
            return Ok();
        else
            return NotFound();
    }
}
