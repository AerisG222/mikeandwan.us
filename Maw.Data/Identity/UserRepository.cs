using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;
using Maw.Data.EntityFramework.Identity;


namespace Maw.Data.Identity
{
    public class UserRepository
        : IUserRepository
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


        public Task<List<State>> GetStatesAsync()
        {
            return _ctx
                .state
                .Select(x => new State {
                    Id = x.id,
                    Code = x.code,
                    Name = x.name
                })
                .ToListAsync();
        }


        public Task<List<Country>> GetCountriesAsync()
        {
            return _ctx
                .country
                .Select(x => new Country {
                    Id = x.id,
                    Code = x.code,
                    Name = x.name
                })
				.ToListAsync();
        }


        public async Task<MawUser> GetUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("Username cannot be null or empty", nameof(username));
			}

			var user = await _ctx.user
                .Include(u => u.state)
                .Include(u => u.country)
				.Include(u => u.user_role).ThenInclude(ur => ur.role)
                .Where(x => x.username == username.ToLower())
                .SingleOrDefaultAsync();

			return BuildMawUser(user);
        }


        public async Task<MawUser> GetUserAsync(short id)
        {
            var user = await _ctx.user
                .Include(u => u.state)
                .Include(u => u.country)
				.Include(u => u.user_role).ThenInclude(ur => ur.role)
                .Where(x => x.id == id)
                .SingleOrDefaultAsync();

			return BuildMawUser(user);
        }


		public async Task<MawUser> GetUserByEmailAsync(string email)
		{
			if(string.IsNullOrEmpty(email))
			{
				throw new ArgumentException("email must not be null or empty", nameof(email));
			}

			var user = await _ctx.user
				.Include(u => u.state)
                .Include(u => u.country)
				.Include(u => u.user_role).ThenInclude(ur => ur.role)
				.Where(x => x.email == email.ToLower())
				.SingleOrDefaultAsync();

			return BuildMawUser(user);
		}


        public async Task<List<string>> GetRoleNamesForUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

            var roles = await _ctx.user
                .Include(u => u.user_role).ThenInclude(r => r.role)
                .Where(x => x.username == username.ToLower())
                .Select(x => x.user_role)
                .SingleOrDefaultAsync();

            return roles.Select(x => x.role.name)
                .OrderBy(x => x)
                .ToList();
        }


		public async Task<bool> UpdateUserPasswordAsync(MawUser user)
		{
			if(user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var u = await _ctx.user.SingleAsync(x => x.username == user.Username.ToLower());

			u.hashed_password = user.HashedPassword;
			u.salt = null;

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


        public async Task<bool> UpdateUserAsync(MawUser updatedUser)
        {
			if(updatedUser == null)
			{
				throw new ArgumentNullException(nameof(updatedUser));
			}

            var user = await _ctx.user.SingleOrDefaultAsync(x => x.username == updatedUser.Username.ToLower());

			if(user == null)
			{
				throw new InvalidOperationException("Was not able to find user to update with username: " + updatedUser.Username.ToLower());
			}

            user.first_name = updatedUser.FirstName;
            user.last_name = updatedUser.LastName;
            user.email = updatedUser.Email.ToLower();
            user.website = updatedUser.Website;
            user.date_of_birth = updatedUser.DateOfBirth;
            user.company_name = updatedUser.Company;
            user.position = updatedUser.Position;
            user.work_email = updatedUser.WorkEmail.ToLower();
            user.address_1 = updatedUser.Address1;
            user.address_2 = updatedUser.Address2;
            user.city = updatedUser.City;
			user.state_id = (await _ctx.state.SingleOrDefaultAsync(x => x.code == updatedUser.State.ToUpper()))?.id;
            user.postal_code = updatedUser.PostalCode;
			user.country_id = (await _ctx.country.SingleOrDefaultAsync(x => x.code == updatedUser.Country.ToUpper()))?.id;
            user.home_phone = updatedUser.HomePhone;
            user.mobile_phone = updatedUser.MobilePhone;
            user.work_phone = updatedUser.WorkPhone;
			user.hashed_password = updatedUser.HashedPassword;
			user.security_stamp = updatedUser.SecurityStamp;
			user.salt = updatedUser.Salt;

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

            var loginHistory = new login_history {
                user_id = userId,
                username = username.ToLower(),
                login_activity_type_id = loginActivityTypeId,
                login_area_id = loginAreaId,
                attempt_time = DateTime.Now,
            };

            _ctx.login_history.Add(loginHistory);

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


        public Task<List<UserAndLastLogin>> GetUsersToManageAsync()
        {
            return _ctx.user
                .Select(x => new UserAndLastLogin {
                    UserId = x.id,
                    Username = x.username,
                    FirstName = x.first_name,
                    LastName = x.last_name,
                    LastLoginDate = _ctx.login_history
                        .Where(u => u.user_id == x.id && u.login_activity_type_id == LOGIN_SUCCESS)
                        .Max(u => u.attempt_time)
                    })
                .OrderBy(x => x.Username)
                .ToListAsync();
        }


        public Task<List<string>> GetAllUsernamesAsync()
        {
            return _ctx.user
                .Select(x => x.username)
                .OrderBy(x => x)
                .ToListAsync();
        }


        public Task<List<string>> GetAllRoleNamesAsync()
        {
            return _ctx.role
                .Select(x => x.name)
                .OrderBy(x => x)
                .ToListAsync();
        }


        public async Task<int> AddUserAsync(MawUser user)
        {
			if(user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			// password and salt are for legacy login module, so we null these out as they are no longer needed
            var u = new user { 
                username = user.Username.ToLower(),
                salt = null,
				hashed_password = user.HashedPassword,
                first_name = user.FirstName,
                last_name = user.LastName,
                email = user.Email.ToLower(),
				security_stamp = user.SecurityStamp
            };

            _ctx.user.Add(u);

			try
			{
				var result = await _ctx.SaveChangesAsync();

				user.Id = u.id;

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

            var user = await _ctx.user.SingleAsync(x => x.username == username.ToLower());
            var roles = user.user_role.ToList();

            foreach(var role in roles)
            {
                user.user_role.Remove(role);
            }

            _ctx.user.Remove(user);

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


        public async Task<List<MawUser>> GetUsersInRoleAsync(string roleName)
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

            var users = await _ctx.user_role
                .Include(ur => ur.role)
                .Include(ur => ur.user).ThenInclude(u => u.country)
				.Include(ur => ur.user).ThenInclude(u => u.state)
                .Where(ur => ur.role.name == roleName.ToLower())
                .Select(ur => ur.user)
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

            var role = new role {
                name = roleName.ToLower(),
                description = description
            };

            _ctx.role.Add(role);

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

            var role = await _ctx.role.SingleAsync(x => x.name == roleName.ToLower());
            var users = role.user_role.ToList();

            foreach(var user in users)
            {
                role.user_role.Remove(user);
            }

            _ctx.role.Remove(role);

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

            var user = _ctx.user.Single(x => x.username == username.ToLower());
            var role = _ctx.role.Single(x => x.name == roleName.ToLower());
            var userRole = new user_role { user_id = user.id, role_id = role.id };
            
            role.user_role.Add(userRole);

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

            var user = await _ctx.user.SingleAsync(x => x.username == username.ToLower());
            var role = await _ctx.role.SingleAsync(x => x.name == roleName.ToLower());
            var userRole = new user_role { user_id = user.id, role_id = role.id };
            
            role.user_role.Remove(userRole);

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

			var user = await _ctx.user.SingleAsync(x => x.username == username.ToLower());

			user.security_stamp = securityStamp;

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


		public async Task<MawRole> GetRoleAsync(string roleName)
		{
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			var result = await _ctx.role
				.SingleOrDefaultAsync(x => x.name == roleName.ToLower());

			if(result == null)
			{
				return null;
			}

			return new MawRole {
				Id = result.id,
				Name = result.name,
				Description = result.description
			};
		}


		public async Task<MawRole> GetRoleAsync(short id)
		{
			var result = await _ctx.role
				.SingleOrDefaultAsync(x => x.id == id);

			if(result == null)
			{
				return null;
			}

			return new MawRole {
				Id = result.id,
				Name = result.name,
				Description = result.description
			};
		}


        MawUser BuildMawUser(user user)
        {
			if(user == null)
			{
				return null;
			}

			var u = new MawUser() {
                Email = user.email,
                Username = user.username,
                Id = user.id,
                FirstName = user.first_name,
                LastName = user.last_name,
                DateOfBirth = user.date_of_birth,
                HomePhone = user.home_phone,
                WorkPhone = user.work_phone,
                MobilePhone = user.mobile_phone,
                Company = user.company_name,
                Position = user.position,
                WorkEmail = user.work_email,
                Address1 = user.address_1,
                Address2 = user.address_2,
                City = user.city,
                State = user.state?.code,
                PostalCode = user.postal_code,
                Country = user.country?.code,
                Website = user.website,
				HashedPassword = user.hashed_password,
				SecurityStamp = user.security_stamp,
				Salt = user.salt
            };

			foreach(var userRole in user.user_role)
			{
				u.AddRole(userRole.role.name);
			}

			return u;
        }


        user BuildUser(MawUser user)
        {
			if(user == null)
			{
				return null;
			}

            return new user() {
                email = user.Email,
                username = user.Username,
                id = Convert.ToInt16(user.Id),
                first_name = user.FirstName,
                last_name = user.LastName,
                date_of_birth = user.DateOfBirth,
                home_phone = user.HomePhone,
                work_phone = user.WorkPhone,
                mobile_phone = user.MobilePhone,
                company_name = user.Company,
                position = user.Position,
                work_email = user.WorkEmail,
                address_1 = user.Address1,
                address_2 = user.Address2,
                city = user.City,
                state = _ctx.state.Single(x => x.code == user.State.ToUpper()),
                postal_code = user.PostalCode,
                country = _ctx.country.Single(x => x.code == user.Country.ToUpper()),
                website = user.Website,
				hashed_password = user.HashedPassword,
				security_stamp = user.SecurityStamp,
				salt = user.Salt
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

