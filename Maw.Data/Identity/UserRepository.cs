using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using D = Maw.Domain.Identity;
using Maw.Data.EntityFramework.Identity;


namespace Maw.Data.Identity
{
    public class UserRepository
        : D.IUserRepository
    {
        const int LOGIN_SUCCESS = 1;

		readonly IdentityContext _ctx;
		readonly ILogger _log;


		public UserRepository(IdentityContext context, ILoggerFactory loggerFactory)
        {
			if(context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if(loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}

			_ctx = context;
			_log = loggerFactory.CreateLogger<UserRepository>();
        }


        public Task<List<D.State>> GetStatesAsync()
        {
            return _ctx
                .State
                .Select(x => new D.State 
				{
					Id = x.Id,
					Code = x.Code,
					Name = x.Name
				})
                .ToListAsync();
        }


        public Task<List<D.Country>> GetCountriesAsync()
        {
            return _ctx
                .Country
                .Select(x => new D.Country 
				{
					Id = x.Id,
					Code = x.Code,
					Name = x.Name
				})
				.ToListAsync();
        }


        public async Task<D.MawUser> GetUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("Username cannot be null or empty", nameof(username));
			}

			username = username.ToLower();

			var user = await _ctx.User
                .Include(u => u.State)
                .Include(u => u.Country)
				.Include(u => u.UserRole).ThenInclude(ur => ur.Role)
                .Where(x => x.Username == username)
                .SingleOrDefaultAsync();

			return BuildMawUser(user);
        }


        public async Task<D.MawUser> GetUserAsync(short id)
        {
            var user = await _ctx.User
                .Include(u => u.State)
                .Include(u => u.Country)
				.Include(u => u.UserRole).ThenInclude(ur => ur.Role)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

			return BuildMawUser(user);
        }


		public async Task<D.MawUser> GetUserByEmailAsync(string email)
		{
			if(string.IsNullOrEmpty(email))
			{
				throw new ArgumentException("email must not be null or empty", nameof(email));
			}

			email = email.ToLower();

			var user = await _ctx.User
				.Include(u => u.State)
                .Include(u => u.Country)
				.Include(u => u.UserRole).ThenInclude(ur => ur.Role)
				.Where(x => x.Email == email)
				.SingleOrDefaultAsync();

			return BuildMawUser(user);
		}


        public async Task<List<string>> GetRoleNamesForUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			username = username.ToLower();

            var roles = await _ctx.User
                .Include(u => u.UserRole).ThenInclude(r => r.Role)
                .Where(x => x.Username == username)
                .Select(x => x.UserRole)
                .SingleOrDefaultAsync();

            return roles.Select(x => x.Role.Name)
                .OrderBy(x => x)
                .ToList();
        }


		public async Task<bool> UpdateUserPasswordAsync(D.MawUser user)
		{
			if(user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var username = user.Username.ToLower();

			var u = await _ctx.User.SingleAsync(x => x.Username == username);

			u.HashedPassword = user.HashedPassword;
			u.Salt = null;

			_ctx.Entry(u).State = EntityState.Modified;

			try
			{
				var result = (await _ctx.SaveChangesAsync()) == 1;

				return result;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(UpdateUserPasswordAsync), ex);
				throw;
			}
		}


        public async Task<bool> UpdateUserAsync(D.MawUser updatedUser)
        {
			if(updatedUser == null)
			{
				throw new ArgumentNullException(nameof(updatedUser));
			}

			var username = updatedUser.Username.ToLower();

            var user = await _ctx.User.SingleOrDefaultAsync(x => x.Username == username);

			if(user == null)
			{
				throw new InvalidOperationException("Was not able to find user to update with username: " + updatedUser.Username.ToLower());
			}

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email.ToLower();
            user.Website = updatedUser.Website;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.CompanyName = updatedUser.Company;
            user.Position = updatedUser.Position;
            user.WorkEmail = updatedUser.WorkEmail.ToLower();
            user.Address1 = updatedUser.Address1;
            user.Address2 = updatedUser.Address2;
            user.City = updatedUser.City;
			user.StateId = (await _ctx.State.SingleOrDefaultAsync(x => x.Code == updatedUser.State.ToUpper()))?.Id;
            user.PostalCode = updatedUser.PostalCode;
			user.CountryId = (await _ctx.Country.SingleOrDefaultAsync(x => x.Code == updatedUser.Country.ToUpper()))?.Id;
            user.HomePhone = updatedUser.HomePhone;
            user.MobilePhone = updatedUser.MobilePhone;
            user.WorkPhone = updatedUser.WorkPhone;
			user.HashedPassword = updatedUser.HashedPassword;
			user.SecurityStamp = updatedUser.SecurityStamp;
			user.Salt = updatedUser.Salt;

			try
			{
                return await _ctx.SaveChangesAsync() == 1;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(UpdateUserAsync), ex);
				throw;
			}
        }


        public async Task<bool> AddLoginHistoryAsync(string username, short loginActivityTypeId, short loginAreaId)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			short? userId = null;

            var loginHistory = new LoginHistory 
			{
                UserId = userId,
            	Username = username.ToLower(),
                LoginActivityTypeId = loginActivityTypeId,
                LoginAreaId = loginAreaId,
                AttemptTime = DateTime.Now,
            };

            _ctx.LoginHistory.Add(loginHistory);

			try
			{
                await _ctx.SaveChangesAsync();

				return true;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(AddLoginHistoryAsync), ex);
				throw;
			}
        }


        public Task<List<D.UserAndLastLogin>> GetUsersToManageAsync()
        {
            return _ctx.User
                .Select(x => new D.UserAndLastLogin 
				{
					UserId = x.Id,
					Username = x.Username,
					FirstName = x.FirstName,
					LastName = x.LastName,
					LastLoginDate = _ctx.LoginHistory
						.Where(u => u.UserId == x.Id && u.LoginActivityTypeId == LOGIN_SUCCESS)
						.Max(u => u.AttemptTime)
				})
                .OrderBy(x => x.Username)
                .ToListAsync();
        }


        public Task<List<string>> GetAllUsernamesAsync()
        {
            return _ctx.User
                .Select(x => x.Username)
                .OrderBy(x => x)
                .ToListAsync();
        }


        public Task<List<string>> GetAllRoleNamesAsync()
        {
            return _ctx.Role
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToListAsync();
        }


        public async Task<int> AddUserAsync(D.MawUser user)
        {
			if(user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			// password and salt are for legacy login module, so we null these out as they are no longer needed
            var u = new User 
			{ 
                Username = user.Username.ToLower(),
                Salt = null,
				HashedPassword = user.HashedPassword,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email.ToLower(),
				SecurityStamp = user.SecurityStamp
            };

            _ctx.User.Add(u);

			try
			{
				var result = await _ctx.SaveChangesAsync();

				user.Id = u.Id;

				return result;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(AddUserAsync), ex);
				throw;
			}
        }


        public async Task<int> RemoveUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			username = username.ToLower();

            var user = await _ctx.User.SingleAsync(x => x.Username == username);
            var roles = user.UserRole.ToList();

            foreach(var role in roles)
            {
                user.UserRole.Remove(role);
            }

            _ctx.User.Remove(user);

			try
			{
                return await _ctx.SaveChangesAsync();
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(RemoveUserAsync), ex);
				throw;
			}
        }


        public async Task<List<D.MawUser>> GetUsersInRoleAsync(string roleName)
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			roleName = roleName.ToLower();

            var users = await _ctx.UserRole
                .Include(ur => ur.Role)
                .Include(ur => ur.User).ThenInclude(u => u.Country)
				.Include(ur => ur.User).ThenInclude(u => u.State)
                .Where(ur => ur.Role.Name == roleName)
                .Select(ur => ur.User)
                .ToListAsync();

			return users.Select(x => BuildMawUser(x))
                .OrderBy(x => x.Username)
                .ToList();
        }


        public async Task<bool> CreateRoleAsync(string roleName, string description)
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

            var role = new Role 
			{
                Name = roleName.ToLower(),
                Description = description
            };

            _ctx.Role.Add(role);

			try
			{
            	return await _ctx.SaveChangesAsync() == 1;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(CreateRoleAsync), ex);
				throw;
			}
        }


        public async Task<bool> RemoveRoleAsync(string roleName)
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			roleName = roleName.ToLower();

            var role = await _ctx.Role.SingleAsync(x => x.Name == roleName);
            var users = role.UserRole.ToList();

            foreach(var user in users)
            {
                role.UserRole.Remove(user);
            }

            _ctx.Role.Remove(role);

			try
			{
                return await _ctx.SaveChangesAsync() == 1;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(RemoveRoleAsync), ex);
				throw;
			}
        }


        public async Task<bool> AddUserToRoleAsync(string username, string roleName)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			username = username.ToLower();
			roleName = roleName.ToLower();

            var user = _ctx.User.Single(x => x.Username == username);
            var role = _ctx.Role.Single(x => x.Name == roleName);
            var userRole = new UserRole 
			{ 
				UserId = user.Id, 
				RoleId = role.Id 
			};
            
            role.UserRole.Add(userRole);

			try
			{
                return await _ctx.SaveChangesAsync() == 1;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(AddUserToRoleAsync), ex);
				throw;
			}
        }


        public async Task<bool> RemoveUserFromRoleAsync(string username, string roleName)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			username = username.ToLower();
			roleName = roleName.ToLower();

            var user = await _ctx.User.SingleAsync(x => x.Username == username);
            var role = await _ctx.Role.SingleAsync(x => x.Name == roleName);
            var userRole = new UserRole 
			{ 
				UserId = user.Id, 
				RoleId = role.Id 
			};
            
            role.UserRole.Remove(userRole);

			try
			{
                return await _ctx.SaveChangesAsync() == 1;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(RemoveUserFromRoleAsync), ex);

				throw;
			}
        }


		public async Task<bool> SetSecurityStampAsync(string username, string securityStamp)
		{
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			username = username.ToLower();

			var user = await _ctx.User.SingleAsync(x => x.Username == username);

			user.SecurityStamp = securityStamp;

			try
			{
				return await _ctx.SaveChangesAsync() == 1;
			}
			catch(DbUpdateException ex)
			{
				LogEntityFrameworkError(nameof(SetSecurityStampAsync), ex);
				throw;
			}
		}


		public async Task<D.MawRole> GetRoleAsync(string roleName)
		{
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			roleName = roleName.ToLower();

			var result = await _ctx.Role.SingleOrDefaultAsync(x => x.Name == roleName);

			if(result == null)
			{
				return null;
			}

			return new D.MawRole 
			{
				Id = result.Id,
				Name = result.Name,
				Description = result.Description
			};
		}


		public async Task<D.MawRole> GetRoleAsync(short id)
		{
			var result = await _ctx.Role
				.SingleOrDefaultAsync(x => x.Id == id);

			if(result == null)
			{
				return null;
			}

			return new D.MawRole 
			{
				Id = result.Id,
				Name = result.Name,
				Description = result.Description
			};
		}


        D.MawUser BuildMawUser(User user)
        {
			if(user == null)
			{
				return null;
			}

			var u = new D.MawUser 
			{
                Email = user.Email,
                Username = user.Username,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                HomePhone = user.HomePhone,
                WorkPhone = user.WorkPhone,
                MobilePhone = user.MobilePhone,
                Company = user.CompanyName,
                Position = user.Position,
                WorkEmail = user.WorkEmail,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                State = user.State?.Code,
                PostalCode = user.PostalCode,
                Country = user.Country?.Code,
                Website = user.Website,
				HashedPassword = user.HashedPassword,
				SecurityStamp = user.SecurityStamp,
				Salt = user.Salt
            };

			foreach(var userRole in user.UserRole)
			{
				u.AddRole(userRole.Role.Name);
			}

			return u;
        }


        User BuildUser(D.MawUser user)
        {
			if(user == null)
			{
				return null;
			}

            return new User 
			{
                Email = user.Email,
                Username = user.Username,
                Id = Convert.ToInt16(user.Id),
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                HomePhone = user.HomePhone,
                WorkPhone = user.WorkPhone,
                MobilePhone = user.MobilePhone,
                CompanyName = user.Company,
                Position = user.Position,
                WorkEmail = user.WorkEmail,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                State = _ctx.State.Single(x => x.Code == user.State.ToUpper()),
                PostalCode = user.PostalCode,
                Country = _ctx.Country.Single(x => x.Code == user.Country.ToUpper()),
                Website = user.Website,
				HashedPassword = user.HashedPassword,
				SecurityStamp = user.SecurityStamp,
				Salt = user.Salt
            };
        }


		void LogEntityFrameworkError(string method, DbUpdateException ex)
		{
			_log.LogError(string.Format("Error calling {0}: {1}", method, ex.Message));

            /*
			foreach(var dbErr in ex.Entries)
			{
				foreach(var err in dbErr.ValidationErrors)
				{
					_log.LogError(string.Format("Error with property [{0}]: {1}", err.PropertyName, err.ErrorMessage));
				}
			}
            */
		}
    }
}

