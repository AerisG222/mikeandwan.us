using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Blogs;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Domain.Utilities;
using MawAuth.ViewModels.Admin;
using MawAuth.ViewModels.Email;
using Mvc.RenderViewToString;
using Maw.Security;


namespace MawAuth.Controllers
{
    [Authorize(MawPolicy.AdminSite)]
    [Route("admin")]
    public class AdminController
        : Controller
    {
        readonly ILogger _log;
        readonly IUserRepository _repo;
        readonly UserManager<MawUser> _userMgr;
        readonly RoleManager<MawRole> _roleMgr;
        readonly IBlogService _blogSvc;
        readonly IEmailService _emailSvc;
        readonly RazorViewToStringRenderer _razorRenderer;
        readonly IPasswordValidator<MawUser> _pwdValidator;


        public AdminController(ILogger<AdminController> log,
                               IUserRepository userRepository,
                               UserManager<MawUser> userManager,
                               RoleManager<MawRole> roleManager,
                               IBlogService blogService,
                               IEmailService emailService,
                               RazorViewToStringRenderer razorRenderer,
                               IPasswordValidator<MawUser> passwordValidator)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleMgr = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _blogSvc = blogService ?? throw new ArgumentNullException(nameof(blogService));
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
            var result = await _repo.GetUsersToManageAsync().ConfigureAwait(false);

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
            return View(await _repo.GetAllRoleNamesAsync().ConfigureAwait(false));
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

                var password = await GeneratePassword().ConfigureAwait(false);

                model.Result = IdentityResult.Failed();

                try
                {
                    model.Result = await _userMgr.CreateAsync(user, password).ConfigureAwait(false);

                    if (model.Result == IdentityResult.Success)
                    {
                        var emailModel = new CreateUserEmailModel
                        {
                            Title = "User Account Created",
                            Username = model.Username,
                            FirstName = model.FirstName,
                            Password = password
                        };

                        var body = await _razorRenderer.RenderViewToStringAsync("~/Views/Email/CreateUser.cshtml", emailModel).ConfigureAwait(false);

                        await _emailSvc.SendHtmlAsync(model.Email, _emailSvc.FromAddress, "Account Created for mikeandwan.us", body).ConfigureAwait(false);
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
                    var user = await _userMgr.FindByNameAsync(model.Username).ConfigureAwait(false);

                    try
                    {
                        model.Result = await _userMgr.DeleteAsync(user).ConfigureAwait(false);
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

                model.Result = await _roleMgr.CreateAsync(role).ConfigureAwait(false);
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
                    var r = await _roleMgr.FindByNameAsync(model.Role).ConfigureAwait(false);
                    model.Result = await _roleMgr.DeleteAsync(r).ConfigureAwait(false);

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

            var user = await _userMgr.FindByNameAsync(id).ConfigureAwait(false);

            var model = new EditProfileModel
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
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
                var user = await _userMgr.FindByNameAsync(model.Username).ConfigureAwait(false);

                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    await _userMgr.UpdateSecurityStampAsync(user).ConfigureAwait(false);
                }

                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                model.Result = await _userMgr.UpdateAsync(user).ConfigureAwait(false);
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

            var user = await _userMgr.FindByNameAsync(id).ConfigureAwait(false);
            var userRoles = await _userMgr.GetRolesAsync(user).ConfigureAwait(false);

            var model = new ManageRolesForUserModel {
                Username = id
            };

            model.AllRoles.AddRange(await _repo.GetAllRoleNamesAsync().ConfigureAwait(false));
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

            string username = collection["Username"];
            var model = new ManageRolesForUserModel
            {
                Username = username
            };

            model.AllRoles.AddRange(await _repo.GetAllRoleNamesAsync().ConfigureAwait(false));

            var user = await _userMgr.FindByNameAsync(username).ConfigureAwait(false);
            var currRoles = await _userMgr.GetRolesAsync(user).ConfigureAwait(false);
            var newRoleList = new List<string>();

            if (!string.IsNullOrEmpty(collection["role"]))
            {
                newRoleList.AddRange(collection["role"]);
            }

            var toRemove = currRoles.Where(cm => !newRoleList.Any(nm => string.Equals(cm, nm, StringComparison.OrdinalIgnoreCase)));
            var toAdd = newRoleList.Where(nm => !currRoles.Any(cm => string.Equals(cm, nm, StringComparison.OrdinalIgnoreCase)));
            var errs = new List<IdentityError>();

            _log.LogInformation("new roles: " + string.Join(", ", newRoleList));

            foreach (var role in toRemove)
            {
                var result = await _userMgr.RemoveFromRoleAsync(user, role).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    errs.AddRange(result.Errors);
                }
            }

            foreach (var role in toAdd)
            {
                var result = await _userMgr.AddToRoleAsync(user, role).ConfigureAwait(false);

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

            // after the changes, get the new membership info (we are now
            user = await _userMgr.FindByNameAsync(username).ConfigureAwait(false);
            currRoles = await _userMgr.GetRolesAsync(user).ConfigureAwait(false);
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

            model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role).ConfigureAwait(false)).Select(x => x.Username);
            model.AllUsers = await _repo.GetAllUsernamesAsync().ConfigureAwait(false);

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
                var currentMembers = await _userMgr.GetUsersInRoleAsync(model.Role).ConfigureAwait(false);
                model.AllUsers = await _repo.GetAllUsernamesAsync().ConfigureAwait(false);

                var toRemove = currentMembers.Where(cm => !model.NewMembers.Any(nm => string.Equals(cm.Username, nm, StringComparison.OrdinalIgnoreCase)));
                var toAdd = model.NewMembers.Where(nm => !currentMembers.Any(cm => string.Equals(cm.Username, nm, StringComparison.OrdinalIgnoreCase)));
                var errs = new List<IdentityError>();

                foreach (var newMember in toAdd)
                {
                    var newUser = await _userMgr.FindByNameAsync(newMember).ConfigureAwait(false);
                    var result = await _userMgr.AddToRoleAsync(newUser, model.Role).ConfigureAwait(false);

                    if (!result.Succeeded)
                    {
                        errs.AddRange(result.Errors);
                    }
                }

                foreach (var oldMember in toRemove)
                {
                    _log.LogInformation("Removing user {Username} from role {Role}", oldMember.Username, model.Role);
                    var result = await _userMgr.RemoveFromRoleAsync(oldMember, model.Role).ConfigureAwait(false);

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

            model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role).ConfigureAwait(false)).Select(x => x.Username);

            return View(model);
        }


        protected void LogValidationErrors()
        {
            var errs = ModelState.Values.SelectMany(v => v.Errors);

            foreach (var err in errs)
            {
                _log.LogWarning(err.ErrorMessage);
            }
        }


        async Task<string> GeneratePassword()
        {
            // limit to 100 tries
            for (int i = 0; i < 100; i++)
            {
                var password = CryptoUtils.GeneratePassword(12);
                var isValid = await _pwdValidator.ValidateAsync(_userMgr, null, password).ConfigureAwait(false);

                if (isValid == IdentityResult.Success)
                {
                    return password;
                }
            }

            throw new InvalidOperationException();
        }
    }
}
