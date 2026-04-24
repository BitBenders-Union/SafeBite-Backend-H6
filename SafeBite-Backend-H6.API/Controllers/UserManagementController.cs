using SafeBite_Backend_H6.API.Contracts.Requests.Role;

namespace SafeBite_Backend_H6.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/[controller]")]
[ApiController]
public class UserManagementController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UserManagementController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsersWithRoles([FromQuery] PaginationParameters parameters, [FromQuery] string? searchTerm = null)
    {
        var usersWithRoles = await _userManagementService.GetUsersPagedAsync(parameters, searchTerm);

        return Ok(usersWithRoles);
    }

    [HttpPost("{id}/A ctivate")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        try
        {
            await _userManagementService.ActivateUserAsync(id);
            return NoContent();
        }

        catch(KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        catch(InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("{id}/Deactivate")]
    public async Task<IActionResult> DeActivateUser(string id)
    {
        try
        {
            await _userManagementService.DeactivateUserAsync(id);
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

    [HttpGet("/GetAllRoles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var result = await _userManagementService.GetAllRolesAsync();

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("/SetRole")]
    public async Task<IActionResult> SetUserRole(AssignRoleRequest request)
    {
        try
        {
            await _userManagementService.AssignRoleAsync(request.UserID, request.RoleName);
            return Ok();
        }
        catch(KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("/RemoveRole")]
    public async Task<IActionResult> RemoveUserRole(AssignRoleRequest request)
    {
        try
        {
            await _userManagementService.RemoveRoleAsync(request.UserID, request.RoleName);
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

