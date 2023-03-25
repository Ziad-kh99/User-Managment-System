namespace UserManagementWithIdentity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if(user is null)
        {
            return NotFound();
        }

        //> Delete User:
        var result = await _userManager.DeleteAsync(user);

        if(!result.Succeeded)
        {
            throw new Exception();
        }


        return Ok();
    }

}