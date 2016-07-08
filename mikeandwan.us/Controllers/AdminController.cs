using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using MawMvcApp.ViewModels.Navigation;


namespace MawMvcApp.Controllers
{
	// TODO: fixup routes to not use querystrings
	[Authorize(MawConstants.POLICY_ADMIN_SITE)]
	[Route("admin")]
    public class AdminController 
        : MawBaseController<AdminController>
    {
        readonly IUserRepository _repo;
		readonly EmailConfig _emailConfig;
		readonly UserManager<MawUser> _userMgr;
		readonly RoleManager<MawRole> _roleMgr;
		readonly IBlogRepository _blogRepo;
		readonly IEmailService _emailSvc;


		public AdminController(IAuthorizationService authorizationService, 
		                       ILogger<AdminController> log, 
							   IOptions<EmailConfig> emailOpts, 
							   IUserRepository userRepository,  
		                       UserManager<MawUser> userManager, 
							   RoleManager<MawRole> roleManager, 
							   IBlogRepository blogRepository, 
							   IEmailService emailService)
			: base(authorizationService, log)
        {
			if(emailOpts == null)
			{
				throw new ArgumentNullException(nameof(emailOpts));
			}
			
			if(userRepository == null)
			{
				throw new ArgumentNullException(nameof(userRepository));
			}

			if(userManager == null)
			{
				throw new ArgumentNullException(nameof(userManager));
			}

			if(roleManager == null)
			{
				throw new ArgumentNullException(nameof(roleManager));
			}

			if (blogRepository == null)
			{
				throw new ArgumentNullException(nameof(blogRepository));
			}
			
			if (emailService == null)
			{
				throw new ArgumentNullException(nameof(emailService));
			}
			
			_emailConfig = emailOpts.Value;
            _repo = userRepository;
			_userMgr = userManager;
			_roleMgr = roleManager;
			_blogRepo = blogRepository;
			_emailSvc = emailService;
        }


		[HttpGet("")]
        public ActionResult Index()
        {
			ViewBag.NavigationZone = NavigationZone.Administration;

            return View();
        }
		
		
		[HttpGet("manage-users")]
		public async Task<ActionResult> ManageUsers()
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
		public async Task<ActionResult> ManageRoles()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			return View(await _repo.GetAllRoleNamesAsync());
		}
		
		
		[HttpGet("create-user")]
		public ActionResult CreateUser()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;
			
            return View(new CreateUserModel());
		}
		
		
		[HttpPost("create-user")]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateUser(CreateUserModel model)
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
				
				try
				{
					model.Result = await _userMgr.CreateAsync(user, password);
					
					var msg = new StringBuilder();
					
					msg.Append("Hello ").Append(model.FirstName).AppendLine(",")
					   .AppendLine()
					   .AppendLine("A user account has been created for you at https://www.mikeandwan.us.  Please find your login details below.")
					   .AppendLine()
					   .Append("Username: ").Append(model.Username).AppendLine()
                       .Append("Password: ").Append(password).AppendLine()
					   .AppendLine()
					   .AppendLine("If you feel you have received this email in error, please discard.")
					   .AppendLine()
					   .AppendLine("Thanks!");
					   
					await _emailSvc.SendAsync(model.Email, _emailConfig.User, "mikeandwan.us user account", msg.ToString());
				}
				catch(Exception ex)
				{
					_log.LogError("error creating user", ex);
				}
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("delete-user/{id}")]
		public ActionResult DeleteUser(string id)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;
			
			if(string.IsNullOrEmpty(id))
			{
				RedirectToAction("Index");
			}
			
			var model = new DeleteUserModel();
			model.Username = id;
			
			return View(model);
		}
		
		
		[HttpPost("delete-user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(DeleteUserModel model, IFormCollection collection)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
			{			
				if(StringValues.IsNullOrEmpty(collection["delete"]))
				{
					var user = await _userMgr.FindByNameAsync(model.Username);
	
					try
					{
						model.Result = await _userMgr.DeleteAsync(user);
					}
					catch(Exception ex)
					{
						_log.LogError("there was an error deleting the user", ex);
					}
				}
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("create-role")]
		public ActionResult CreateRole()
		{
			ViewBag.NavigationZone = NavigationZone.Administration;
			
			return View(new CreateRoleModel());
		}
		
		
		[HttpPost("create-role")]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateRole(CreateRoleModel model)
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
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("delete-role")]
		public ActionResult DeleteRole(string id)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;
			
			if(string.IsNullOrEmpty(id))
			{
				return RedirectToAction("Index");
			}

			var model = new DeleteRoleModel();
			model.Role = id;
			
			return View(model);
		}
		
		
		[HttpPost("delete-role")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRole(DeleteRoleModel model, IFormCollection collection)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
			{
				if(StringValues.IsNullOrEmpty(collection["delete"]))
				{
					var r = await _roleMgr.FindByNameAsync(model.Role);
					model.Result = await _roleMgr.DeleteAsync(r);
				}
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("edit-profile")]
		public async Task<ActionResult> EditProfile(string id)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;
			
			if(string.IsNullOrEmpty(id))
			{
				return RedirectToAction("Index");
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
				return RedirectToAction("Index");
			}
		}
		
		
		[HttpPost("edit-profile")]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> EditProfile(EditProfileModel model)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;

			if(ModelState.IsValid)
			{
				var user = await _userMgr.FindByNameAsync(model.Username);

				user.Email = model.Email;
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;

                model.Result = await _userMgr.UpdateAsync(user);
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
		
		
		[HttpGet("manage-roles-for-user")]
		public async Task<ActionResult> ManageRolesForUser(string id)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;
			
			if(string.IsNullOrEmpty(id))
			{
				return RedirectToAction("Index");
			}

			var user = await _userMgr.FindByNameAsync(id);
			var userRoles = await _userMgr.GetRolesAsync(user);

			var model = new ManageRolesForUserModel();
			model.Username = id;
            model.AllRoles.AddRange(await _repo.GetAllRoleNamesAsync());
            model.GrantedRoles.AddRange(userRoles);
			
			return View(model);
		}
		
		
		[HttpPost("manage-roles-for-user")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageRolesForUser(IFormCollection collection)
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
		
		
		[HttpGet("edit-role-members")]
		public async Task<ActionResult> EditRoleMembers(string id)
		{
			ViewBag.NavigationZone = NavigationZone.Administration;
			
			if(string.IsNullOrEmpty(id))
			{
				RedirectToAction("Index");
			}
			
			var model = new EditRoleMembersModel();
			
			model.Role = id;

			model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role)).Select(x => x.Username);
            model.AllUsers = await _repo.GetAllUsernamesAsync();
			
			return View(model);
		}
		
		
		[HttpPost("edit-role-members")]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> EditRoleMembers(EditRoleMembersModel model)
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
				LogValidationErrors();
			}

			model.Members = (await _userMgr.GetUsersInRoleAsync(model.Role)).Select(x => x.Username);

			return View(model);
		}
		
		
		[HttpGet("create-blog-post")]
		public ActionResult CreateBlogPost()
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
		public ActionResult CreateBlogPost(BlogPostModel model)
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
					var svc = new BlogService(_blogRepo);
					var post = new Post()
					{
						BlogId = 1,
						Title = model.Title,
						Description = model.Description,
						PublishDate = model.PublishDate
					};
					
					svc.AddPostAsync(post);
					
					model.Success = true;
				}
			}
			else
			{
				LogValidationErrors();
			}
			
			return View(model);
		}
    }
}
