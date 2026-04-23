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
}
