namespace UserManagementWithIdentity.Controllers;


[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<IActionResult> Index()
    {
        //* This is true, but will return users in ApplicationUserModel(get all attributes)
        // var users =  await _userManger.Users.ToListAsync();

        var users = _userManager.Users.Select(user => 
        new UserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            Roles = _userManager.GetRolesAsync(user).Result
        }).ToList();

        return View(users);
    }

    #region Add User:
    public async Task<IActionResult> AddUser()
    {
        //> will send app's roles with model to assign role to created user
        var roles = await _roleManager.Roles
            .Select(r => new RoleViewModel { RoleId = r.Id, RoleName = r.Name})
            .ToListAsync();

        var viewModel = new AddUserViewModel 
        {
            Roles = roles
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        if(!ModelState.IsValid)
            return View(model);
        

        //> Inforce Admin to assgin at least one role for user:
        if(!model.Roles.Any(r => r.IsSelected))
        {
            ModelState.AddModelError("Roles", "Please select at least one role");
            return View(model);
        }

        //> Check Email is unique:
        if(await _userManager.FindByEmailAsync(model.Email) is not null)  // then theres a user with this email
        {
            ModelState.AddModelError("Email", "This email is already exists");
            return View(model);
        }

        //> Check Username is unique:
        if(await _userManager.FindByNameAsync(model.Username) is not null)
        {
            ModelState.AddModelError("Username", "This username is already exists");
            return View(model);
        }


        //> Add new user:
        var newUser = new ApplicationUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.Username,
        };
        var result = await _userManager.CreateAsync(newUser, model.Password);
        if(!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("Roles", error.Description);
            }

            return View(model);
        }


        //> Add selected roles to user:
        await _userManager.AddToRolesAsync(newUser, model.Roles
            .Where(r => r.IsSelected)
            .Select(r => r.RoleName));


        return RedirectToAction(nameof(Index));
    }
    #endregion


    #region Manage Roles:
    public async Task<IActionResult> ManageRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if(user is null)
        {
            return NotFound();
        }

        var roles = await _roleManager.Roles.ToListAsync();

        var viewModel = new UserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.FirstName + " " + user.LastName,
            Roles = roles.Select(role => 
            new RoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);

        if(user is null)
        {
            return NotFound();
        }

        //> Get all roles assigned to user:
        var userRoles = await _userManager.GetRolesAsync(user);

        
        /*/> TODO:
        - Exception: user that is not assigned to any role make 'userRoles == null'
            and then we cannot assign any role to it.

        - We solve it by change condition in Add Role, Becuse Any() returns false immediatly
            if list is empty;
        Last code: userRoles.Any(r => r != role.RoleName);  HERE Any() always returns false 
        Correct code: !userRoles.Any(r => r == role.RoleName);
            
        //> Add user to 'User' role if he has not any role.
        if(userRoles.Count == 0)    // user has not any roles
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        */
        

        //> Loop on all roles in our app(model):
        foreach(var role in model.Roles)
        {
            //> Remove Role: current role is asigned to user and not selected yet then we must remove it:
            if(userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
            {
                await _userManager.RemoveFromRoleAsync(user, role.RoleName);
            }

            //> Add Role: current role is not asigned to user and now is selected, then we must add it:
            if(!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
            {
                await _userManager.AddToRoleAsync(user, role.RoleName);
            }
        }

        return RedirectToAction(nameof(Index));
    }
    #endregion


    #region Edit Profile
    public async Task<IActionResult> EditProfile(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if(user is null)
        {
            return NotFound();
        }

        var viewModel = new EditProfileViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.UserName
        };



        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if(user is null)
        {
            return NotFound();
        }

        #region Check Emial and Username to avoid Duplication:

        // Get email from model:
        var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);

        // Case 1: There is no user with the same email (No problem):
        // Case 2: The emial is assigned to current user (No problem): (2nd condition)
        // Case 3: The email is assigned to another user (Problem): (1st condition)
        if(userWithSameEmail is not null && userWithSameEmail.Id != user.Id)
        {
            ModelState.AddModelError("Email", "This email is assigned to another user");

            return View(model);
        }


        // Get username form model:
        var userWithSameUsername = await _userManager.FindByNameAsync(model.Username);
        if(userWithSameUsername is not null && userWithSameUsername.Id != user.Id)
        {
            ModelState.AddModelError("Username", "This Username is assigned to another user");

            return View(model);
        }


        #endregion


        //> Edit Profile:
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.UserName = model.Username;


        //> Sava Changes in DB:
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    #endregion

}