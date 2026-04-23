namespace SafeBite_Backend_H6.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpGet("total-count")]
    public async Task<IActionResult> GetTotalUserCount()
    {
        var totalUserCount = await _userService.GetTotalUserCount();
        return Ok(totalUserCount);
    }

    [HttpGet("active-count")]
    public async Task<IActionResult> GetTotalActiveUserCount()
    {
        var totalActiveUserCount = await _userService.GetTotalActiveUserCount();
        return Ok(totalActiveUserCount);
    }

    [HttpGet("inactive-count")]
    public async Task<IActionResult> GetTotalInactiveUserCount()
    {
        var totalInactiveUserCount = await _userService.GetTotalInactiveUserCount();
        return Ok(totalInactiveUserCount);
    }

}
