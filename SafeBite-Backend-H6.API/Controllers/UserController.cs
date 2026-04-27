namespace SafeBite_Backend_H6.API.Controllers;

[Authorize(Roles = "Admin")]
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

    [HttpGet]
    public async Task<IActionResult> GetAllUsersWithRoles([FromQuery] PaginationParameters parameters, [FromQuery] string? searchTerm = null)
    {
        var usersWithRoles = await _userService.GetUsersPagedAsync(parameters, searchTerm);

        return Ok(usersWithRoles);
    }

    [HttpPost("UserManagement/{id}/Activate")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        try
        {
            await _userService.ActivateUserAsync(id);
            return NoContent();
        }

        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("UserManagement/{id}/Deactivate")]
    public async Task<IActionResult> DeActivateUser(string id)
    {
        try
        {
            await _userService.DeactivateUserAsync(id);
            return NoContent();
        }

        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("UserManagement/GetAllRoles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var result = await _userService.GetAllRolesAsync();

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("UserManagement/SetRole")]
    public async Task<IActionResult> SetUserRole(AssignRoleRequest request)
    {
        try
        {
            await _userService.AssignRoleAsync(request.UserID, request.RoleName);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("UserManagement/RemoveRole")]
    public async Task<IActionResult> RemoveUserRole(AssignRoleRequest request)
    {
        try
        {
            await _userService.RemoveRoleAsync(request.UserID, request.RoleName);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
