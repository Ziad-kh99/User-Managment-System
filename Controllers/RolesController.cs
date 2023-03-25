namespace UserManagementWithIdentity.Controllers;


[Authorize(Roles = "Admin")]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }


    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();

        return View(roles);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]      // What this???
    public async Task<IActionResult> AddRole(RoleFormViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View("Index", await _roleManager.Roles.ToListAsync());
        }

        var roleIsExists = await _roleManager.RoleExistsAsync(model.RoleName);
        if(roleIsExists)
        {
            ModelState.AddModelError("RoleName", "Role is already exists!");
            return View("Index", await _roleManager.Roles.ToListAsync());
        }

        var newRole = new IdentityRole(model.RoleName.Trim());
        await _roleManager.CreateAsync(newRole);

        return RedirectToAction(nameof(Index));
    }

}