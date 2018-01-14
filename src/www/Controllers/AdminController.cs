using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Maw.Domain.Blogs;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Domain.Utilities;
using MawMvcApp.ViewModels;
using MawMvcApp.ViewModels.Admin;
using MawMvcApp.ViewModels.Email;
using MawMvcApp.ViewModels.Navigation;
using Mvc.RenderViewToString;
using Maw.Security;


namespace MawMvcApp.Controllers
{
	[Authorize(Policy.AdminSite)]
	[Route("admin")]
    public class AdminController
        : MawBaseController<AdminController>
    {
        readonly IUserRepository _repo;
		readonly UserManager<MawUser> _userMgr;
		readonly RoleManager<MawRole> _roleMgr;
		readonly IBlogService _blogSvc;
		readonly IEmailService _emailSvc;
		readonly RazorViewToStringRenderer _razorRenderer;


		public AdminController(ILogger<AdminController> log,
							   IUserRepository userRepository,
		                       UserManager<MawUser> userManager,
							   RoleManager<MawRole> roleManager,
							   IBlogService blogService,
							   IEmailService emailService,
							   RazorViewToStringRenderer razorRenderer)
			: base(log)
        {
            _repo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_userMgr = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_roleMgr = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
			_blogSvc = blogService ?? throw new ArgumentNullException(nameof(blogService));
			_emailSvc = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_razorRenderer = razorRenderer ?? throw new ArgumentNullException(nameof(razorRenderer));
        }


		[HttpGet("")]
        public IActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Administration;

            return View();
        }


		[HttpGet("manage-users")]
		public async Task<IActionResult> ManageUsers()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			var result = await _repo.GetUsersToManageAsync();

			var model = result.Select(x => new ManageUserModel
			{
				Username = x.Username,
				FirstName = x.FirstName,
				LastName = x.LastName,
				LastLoginDate = x.LastLoginDate
			});

            return View(model);
		}


		[HttpGet("manage-roles")]
		public async Task<IActionResult> ManageRoles()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			return View(await _repo.GetAllRoleNamesAsync());
		}


		[HttpGet("create-user")]
		public IActionResult CreateUser()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

            return View(new CreateUserModel());
		}


		[HttpPost("create-user")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateUser(CreateUserModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
			{
				var user = new MawUser
				{
					Username = model.Username,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email
				};

				var crypto = new Crypto();
				var password = crypto.GeneratePassword(12);
				model.Result = IdentityResult.Failed();

				try
				{
					model.Result = await _userMgr.CreateAsync(user, password);

					if(model.Result == IdentityResult.Success)
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
				catch(Exception ex)
				{
					_log.LogError("error creating user", ex);
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
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(string.IsNullOrEmpty(id))
			{
				RedirectToAction(nameof(Index));
			}

			var model = new DeleteUserModel();
			model.Username = id;

			return View(model);
		}


		[HttpPost("delete-user/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(DeleteUserModel model, IFormCollection collection)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
			{
				if(collection.Any(x => string.Equals(x.Key, "delete", StringComparison.OrdinalIgnoreCase)))
				{
					var user = await _userMgr.FindByNameAsync(model.Username);

					try
					{
						model.Result = await _userMgr.DeleteAsync(user);
						return RedirectToAction(nameof(ManageUsers));
					}
					catch(Exception ex)
					{
						_log.LogError("there was an error deleting the user", ex);
					}
				}
				else {
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
			ViewBag.NavigationZone = NavigationZone.Administration;

			return View(new CreateRoleModel());
		}


		[HttpPost("create-role")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateRole(CreateRoleModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
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
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(string.IsNullOrEmpty(id))
			{
				return RedirectToAction(nameof(Index));
			}

			var model = new DeleteRoleModel();
			model.Role = id;

			return View(model);
		}


		[HttpPost("delete-role/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(DeleteRoleModel model, IFormCollection collection)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
			{
				if(collection.Any(x => string.Equals(x.Key, "delete", StringComparison.OrdinalIgnoreCase)))
				{
					var r = await _roleMgr.FindByNameAsync(model.Role);
					model.Result = await _roleMgr.DeleteAsync(r);

					return RedirectToAction(nameof(ManageRoles));
				}
				else {
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
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(string.IsNullOrEmpty(id))
			{
				return RedirectToAction(nameof(Index));
			}

            var user = await _userMgr.FindByNameAsync(id);

			var model = new EditProfileModel
			{
				Username = user.Username,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email
			};

            if(model != null)
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
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
			{
				var user = await _userMgr.FindByNameAsync(model.Username);

				if(string.IsNullOrEmpty(user.SecurityStamp))
				{
					await _userMgr.UpdateSecurityStampAsync(user);
				}

				user.Email = model.Email;
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;

                model.Result = await _userMgr.UpdateAsync(user);
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
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(string.IsNullOrEmpty(id))
			{
				return RedirectToAction(nameof(Index));
			}

			var user = await _userMgr.FindByNameAsync(id);
			var userRoles = await _userMgr.GetRolesAsync(user);

			var model = new ManageRolesForUserModel();
			model.Username = id;
            model.AllRoles.AddRange(await _repo.GetAllRoleNamesAsync());
            model.GrantedRoles.AddRange(userRoles);

			return View(model);
		}


		[HttpPost("manage-roles-for-user/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRolesForUser(IFormCollection collection)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			string username = collection["Username"];
			var model = new ManageRolesForUserModel();

			model.Username = username;
			model.AllRoles.AddRange(await _repo.GetAllRoleNamesAsync());

			var user = await _userMgr.FindByNameAsync(username);
			var currRoles = await _userMgr.GetRolesAsync(user);
			var newRoleList = new List<string>();

			if(!string.IsNullOrEmpty(collection["role"]))
			{
				newRoleList.AddRange(collection["role"]);
			}

			var toRemove = currRoles.Where(cm => !newRoleList.Any(nm => string.Equals(cm, nm, StringComparison.OrdinalIgnoreCase)));
			var toAdd = newRoleList.Where(nm => !currRoles.Any(cm => string.Equals(cm, nm, StringComparison.OrdinalIgnoreCase)));
			var errs = new List<IdentityError>();

			_log.LogInformation("new roles: " + string.Join(", ", newRoleList));

			foreach(var role in toRemove)
			{
				var result = await _userMgr.RemoveFromRoleAsync(user, role);

				if(!result.Succeeded)
				{
					errs.AddRange(result.Errors);
				}
			}

			foreach(var role in toAdd)
			{
				var result = await _userMgr.AddToRoleAsync(user, role);

				if(!result.Succeeded)
				{
					errs.AddRange(result.Errors);
				}
			}

			if(errs.Count() == 0)
			{
				model.Result = IdentityResult.Success;
			}
			else
			{
				model.Result = IdentityResult.Failed(errs.ToArray());
			}

			// after the changes, get the new membership info (we are now
			user = await _userMgr.FindByNameAsync(username);
			currRoles = await _userMgr.GetRolesAsync(user);
			model.GrantedRoles.AddRange(currRoles);

			return View(model);
		}


		[HttpGet("edit-role-members/{id}")]
		public async Task<IActionResult> EditRoleMembers(string id)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(string.IsNullOrEmpty(id))
			{
				RedirectToAction(nameof(Index));
			}

			var model = new EditRoleMembersModel();

			model.Role = id;

			model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role)).Select(x => x.Username);
            model.AllUsers = await _repo.GetAllUsernamesAsync();

			return View(model);
		}


		[HttpPost("edit-role-members/{id}")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> EditRoleMembers(EditRoleMembersModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			// this follows the same logic as ManageRolesForUser, so see that for more info
			if(ModelState.IsValid)
			{
				var currentMembers = await _userMgr.GetUsersInRoleAsync(model.Role);
				model.AllUsers = await _repo.GetAllUsernamesAsync();

				var toRemove = currentMembers.Where(cm => !model.NewMembers.Any(nm => string.Equals(cm.Username, nm, StringComparison.OrdinalIgnoreCase)));
				var toAdd = model.NewMembers.Where(nm => !currentMembers.Any(cm => string.Equals(cm.Username, nm, StringComparison.OrdinalIgnoreCase)));
				var errs = new List<IdentityError>();

				foreach(var newMember in toAdd)
				{
					var newUser = await _userMgr.FindByNameAsync(newMember);
					var result = await _userMgr.AddToRoleAsync(newUser, model.Role);

					if(!result.Succeeded)
					{
						errs.AddRange(result.Errors);
					}
				}

				foreach(var oldMember in toRemove)
				{
					_log.LogInformation(string.Format("removing {0} from {1}", oldMember.Username, model.Role));
					var result = await _userMgr.RemoveFromRoleAsync(oldMember, model.Role);

					if(!result.Succeeded)
					{
						errs.AddRange(result.Errors);
					}
				}

				if(errs.Count() == 0)
				{
					model.Result = IdentityResult.Success;
				}
				else
				{
					model.Result = IdentityResult.Failed(errs.ToArray());
				}
			}
			else
			{
				model.Result = IdentityResult.Failed();
				LogValidationErrors();
			}

			model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role)).Select(x => x.Username);

			return View(model);
		}


		[HttpGet("create-blog-post")]
		public IActionResult CreateBlogPost()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			var post = new BlogPostModel();
			var date = DateTime.Now;

			post.Title = date.ToString("MMMM dd, yyyy");
			post.PublishDate = date;

			return View(post);
		}


		[HttpPost("create-blog-post")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateBlogPost(BlogPostModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if (ModelState.IsValid)
			{
				if(model.Behavior == BlogPostAction.Preview)
				{
					model.Preview = true;
				}
				if (model.Behavior == BlogPostAction.Save)
				{
					model.WasAttempted = true;

					var post = new Post()
					{
						BlogId = 1,
						Title = model.Title,
						Description = model.Description,
						PublishDate = model.PublishDate
					};

					await _blogSvc.AddPostAsync(post);

					model.Success = true;
				}
			}
			else
			{
				model.WasAttempted = true;
				LogValidationErrors();
			}

			return View(model);
		}


		[HttpGet("show-request-details")]
		public IActionResult ShowRequestDetails()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			return View(HttpContext);
		}
    }
}
