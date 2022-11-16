using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Maw.Data.Abstractions;
using Maw.Domain.Email;
using Maw.Domain.Models.Identity;
using Maw.Domain.Utilities;
using MawAuth.ViewModels.Admin;
using MawAuth.ViewModels.Email;
using Mvc.RenderViewToString;
using Maw.Security;

namespace MawAuth.Controllers;

[Authorize(MawPolicy.AdminSite)]
[Route("admin")]
public class AdminController
    : Controller
{
    readonly ILogger _log;
    readonly IUserRepository _repo;
    readonly UserManager<MawUser> _userMgr;
    readonly RoleManager<MawRole> _roleMgr;
    readonly IEmailService _emailSvc;
    readonly RazorViewToStringRenderer _razorRenderer;
    readonly IPasswordValidator<MawUser> _pwdValidator;

    public AdminController(
        ILogger<AdminController> log,
        IUserRepository userRepository,
        UserManager<MawUser> userManager,
        RoleManager<MawRole> roleManager,
        IEmailService emailService,
        RazorViewToStringRenderer razorRenderer,
        IPasswordValidator<MawUser> passwordValidator)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleMgr = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _emailSvc = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _razorRenderer = razorRenderer ?? throw new ArgumentNullException(nameof(razorRenderer));
        _pwdValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("manage-users")]
    public async Task<IActionResult> ManageUsers()
    {
        var result = await _repo.GetUsersToManageAsync();

        var model = result.Select(x => new ManageUserModel
        {
            Username = x.Username,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            LastLoginDate = x.LastLoginDate
        });

        return View(model);
    }

    [HttpGet("manage-roles")]
    public async Task<IActionResult> ManageRoles()
    {
        return View(await _repo.GetAllRoleNamesAsync());
    }

    [HttpGet("create-user")]
    public IActionResult CreateUser()
    {
        return View(new CreateUserModel());
    }

    [HttpPost("create-user")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (ModelState.IsValid)
        {
            var user = new MawUser
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };

            var password = await GeneratePassword();

            model.Result = IdentityResult.Failed();

            try
            {
                model.Result = await _userMgr.CreateAsync(user, password);

                if (model.Result == IdentityResult.Success)
                {
                    var emailModel = new CreateUserEmailModel
                    (
                        "User Account Created",
                        model.Username,
                        model.FirstName,
                        password
                    );

                    var body = await _razorRenderer.RenderViewToStringAsync("~/Views/Email/CreateUser.cshtml", emailModel);

                    await _emailSvc.SendHtmlAsync(model.Email, _emailSvc.FromAddress, "Account Created for mikeandwan.us", body);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "error creating user");
                model.Result = IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }
        else
        {
            model.Result = IdentityResult.Failed();
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("delete-user/{id}")]
    public IActionResult DeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            RedirectToAction(nameof(Index));
        }

        var model = new DeleteUserModel
        {
            Username = id
        };

        return View(model);
    }

    [HttpPost("delete-user/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(DeleteUserModel model, IFormCollection collection)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (ModelState.IsValid)
        {
            if (collection.Any(x => string.Equals(x.Key, "delete", StringComparison.OrdinalIgnoreCase)))
            {
                var user = await _userMgr.FindByNameAsync(model.Username);

                if(user == null)
                {
                    return RedirectToAction(nameof(ManageUsers));
                }

                try
                {
                    model.Result = await _userMgr.DeleteAsync(user);
                    return RedirectToAction(nameof(ManageUsers));
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "there was an error deleting the user");
                }
            }
            else
            {
                return RedirectToAction(nameof(ManageUsers));
            }
        }
        else
        {
            model.Result = IdentityResult.Failed();
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("create-role")]
    public IActionResult CreateRole()
    {
        return View(new CreateRoleModel());
    }

    [HttpPost("create-role")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(CreateRoleModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (ModelState.IsValid)
        {
            var role = new MawRole
            {
                Name = model.Name,
                Description = model.Description
            };

            model.Result = await _roleMgr.CreateAsync(role);
        }
        else
        {
            model.Result = IdentityResult.Failed();
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("delete-role/{id}")]
    public IActionResult DeleteRole(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new DeleteRoleModel
        {
            Role = id
        };

        return View(model);
    }

    [HttpPost("delete-role/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(DeleteRoleModel model, IFormCollection collection)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (ModelState.IsValid)
        {
            if (collection.Any(x => string.Equals(x.Key, "delete", StringComparison.OrdinalIgnoreCase)))
            {
                var r = await _roleMgr.FindByNameAsync(model.Role);

                if(r != null)
                {
                    model.Result = await _roleMgr.DeleteAsync(r);
                }

                return RedirectToAction(nameof(ManageRoles));
            }
            else
            {
                return RedirectToAction(nameof(ManageRoles));
            }
        }
        else
        {
            model.Result = IdentityResult.Failed();
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("edit-profile/{id}")]
    public async Task<IActionResult> EditProfile(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await _userMgr.FindByNameAsync(id);

        if(user == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new EditProfileModel
        {
            Username = user.Username ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpPost("edit-profile/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (ModelState.IsValid)
        {
            var user = await _userMgr.FindByNameAsync(model.Username);

            if (user == null)
            {
                model.Result = IdentityResult.Failed();
                ModelState.AddModelError("Username", "Invalid username");
            }
            else
            {
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    await _userMgr.UpdateSecurityStampAsync(user);
                }

                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                model.Result = await _userMgr.UpdateAsync(user);
            }
        }
        else
        {
            model.Result = IdentityResult.Failed();
            LogValidationErrors();
        }

        return View(model);
    }

    [HttpGet("manage-roles-for-user/{id}")]
    public async Task<IActionResult> ManageRolesForUser(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await _userMgr.FindByNameAsync(id);

        if(user == null)
        {
            return BadRequest();
        }

        var userRoles = await _userMgr.GetRolesAsync(user);

        var model = new ManageRolesForUserModel {
            Username = id
        };

        model.AllRoles.AddRange(await _repo.GetAllRoleNamesAsync());
        model.GrantedRoles.AddRange(userRoles);

        return View(model);
    }

    [HttpPost("manage-roles-for-user/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageRolesForUser(IFormCollection collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        var username = collection["Username"].First();

        if(string.IsNullOrWhiteSpace(username))
        {
            return BadRequest();
        }

        var model = new ManageRolesForUserModel
        {
            Username = username
        };

        model.AllRoles.AddRange(await _repo.GetAllRoleNamesAsync());

        var user = await _userMgr.FindByNameAsync(username);

        if(user == null)
        {
            return BadRequest();
        }

        var currRoles = await _userMgr.GetRolesAsync(user);
        var newRoleList = new List<string>();

        if (!string.IsNullOrEmpty(collection["role"]))
        {
            newRoleList.AddRange(collection["role"]);
        }

        var toRemove = currRoles.Where(cm => !newRoleList.Any(nm => string.Equals(cm, nm, StringComparison.OrdinalIgnoreCase)));
        var toAdd = newRoleList.Where(nm => !currRoles.Any(cm => string.Equals(cm, nm, StringComparison.OrdinalIgnoreCase)));
        var errs = new List<IdentityError>();

        _log.LogInformation("new roles: {Roles}", string.Join(", ", newRoleList));

        foreach (var role in toRemove)
        {
            var result = await _userMgr.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                errs.AddRange(result.Errors);
            }
        }

        foreach (var role in toAdd)
        {
            var result = await _userMgr.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                errs.AddRange(result.Errors);
            }
        }

        if (errs.Any())
        {
            model.Result = IdentityResult.Failed(errs.ToArray());
        }
        else
        {
            model.Result = IdentityResult.Success;
        }

        // after the changes, get the new membership info
        user = await _userMgr.FindByNameAsync(username);
        currRoles = await _userMgr.GetRolesAsync(user ?? throw new InvalidOperationException("user should be found!"));
        model.GrantedRoles.AddRange(currRoles);

        return View(model);
    }

    [HttpGet("edit-role-members/{id}")]
    public async Task<IActionResult> EditRoleMembers(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            RedirectToAction(nameof(Index));
        }

        var model = new EditRoleMembersModel
        {
            Role = id
        };

        model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role))
            .Select(x => x.Username)
            .Where(x => !string.IsNullOrEmpty(x))
            .Cast<string>();
        model.AllUsers = await _repo.GetAllUsernamesAsync();

        return View(model);
    }

    [HttpPost("edit-role-members/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoleMembers(EditRoleMembersModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        // this follows the same logic as ManageRolesForUser, so see that for more info
        if (ModelState.IsValid)
        {
            var currentMembers = await _userMgr.GetUsersInRoleAsync(model.Role);
            model.AllUsers = await _repo.GetAllUsernamesAsync();

            var toRemove = currentMembers.Where(cm => !model.NewMembers.Any(nm => string.Equals(cm.Username, nm, StringComparison.OrdinalIgnoreCase)));
            var toAdd = model.NewMembers.Where(nm => !currentMembers.Any(cm => string.Equals(cm.Username, nm, StringComparison.OrdinalIgnoreCase)));
            var errs = new List<IdentityError>();

            foreach (var newMember in toAdd)
            {
                var newUser = await _userMgr.FindByNameAsync(newMember);

                if(newUser != null)
                {
                    var result = await _userMgr.AddToRoleAsync(newUser, model.Role);

                    if (!result.Succeeded)
                    {
                        errs.AddRange(result.Errors);
                    }
                }
            }

            foreach (var oldMember in toRemove)
            {
                _log.LogInformation("Removing user {Username} from role {Role}", oldMember.Username, model.Role);
                var result = await _userMgr.RemoveFromRoleAsync(oldMember, model.Role);

                if (!result.Succeeded)
                {
                    errs.AddRange(result.Errors);
                }
            }

            if (errs.Any())
            {
                model.Result = IdentityResult.Failed(errs.ToArray());
            }
            else
            {
                model.Result = IdentityResult.Success;
            }
        }
        else
        {
            model.Result = IdentityResult.Failed();
            LogValidationErrors();
        }

        model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role))
            .Select(x => x.Username)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>();

        return View(model);
    }

    protected void LogValidationErrors()
    {
        var errs = ModelState.Values.SelectMany(v => v.Errors);

        foreach (var err in errs)
        {
            _log.LogWarning("Validation error: {ValidationError}", err.ErrorMessage);
        }
    }

    async Task<string> GeneratePassword()
    {
        var dummyUser = new MawUser();

        // limit to 100 tries
        for (int i = 0; i < 100; i++)
        {
            var password = CryptoUtils.GeneratePassword(12);
            var isValid = await _pwdValidator.ValidateAsync(_userMgr, dummyUser, password);

            if (isValid == IdentityResult.Success)
            {
                return password;
            }
        }

        throw new InvalidOperationException();
    }
}
