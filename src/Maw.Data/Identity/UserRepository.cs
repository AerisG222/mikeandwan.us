using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain.Identity;


namespace Maw.Data.Identity
{
    public class UserRepository
        : Repository, IUserRepository
    {
        const int LOGIN_SUCCESS = 1;


		public UserRepository(string connectionString)
			: base(connectionString)
		{

		}


        public Task<IEnumerable<State>> GetStatesAsync()
        {
			return RunAsync(conn => {
				return conn.QueryAsync<State>(
					@"SELECT *
					    FROM maw.state;"
				);
			});
        }


        public Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return RunAsync(conn => {
				return conn.QueryAsync<Country>(
					@"SELECT *
					    FROM maw.country;"
				);
			});
        }


        public Task<MawUser> GetUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("Username cannot be null or empty", nameof(username));
			}

			return GetUserAsync("username", username.ToLower());
        }


        public Task<MawUser> GetUserAsync(short id)
        {
			return GetUserAsync("id", id);
        }


		public Task<MawUser> GetUserByEmailAsync(string email)
		{
			if(string.IsNullOrEmpty(email))
			{
				throw new ArgumentException("email must not be null or empty", nameof(email));
			}

			return GetUserAsync("email", email.ToLower());
		}


        public Task<IEnumerable<string>> GetRoleNamesForUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			return RunAsync(conn => {
				return conn.QueryAsync<string>(
					@"SELECT r.name
					    FROM maw.role r
					   INNER JOIN maw.user_role ur ON ur.role_id = r.id
					   INNER JOIN maw.user u ON u.id = ur.user_id
					   WHERE u.username = @username
					   ORDER BY r.name;",
					new { username = username.ToLower() }
				);
			});
        }


		public Task<bool> UpdateUserPasswordAsync(MawUser user)
		{
			if(user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"UPDATE maw.user
					     SET hashed_password = @hashedPassword
					   WHERE username = @username",
					new { hashedPassword = user.HashedPassword, username = user.Username.ToLower() }
				).ConfigureAwait(false);

				return result == 1;
			});
		}


        public Task<bool> UpdateUserAsync(MawUser updatedUser)
        {
			if(updatedUser == null)
			{
				throw new ArgumentNullException(nameof(updatedUser));
			}

			return RunAsync(async conn => {
				State state = null;
				Country country = null;

				if(!string.IsNullOrWhiteSpace(updatedUser.State))
				{
					state = await GetStateAsync(conn, updatedUser.State);
				}

				if(!string.IsNullOrWhiteSpace(updatedUser.Country))
				{
					country = await GetCountryAsync(conn, updatedUser.Country);
				}

				var result = await conn.ExecuteAsync(
					@"UPDATE maw.user
					     SET first_name = @firstName,
						     last_name = @lastName,
							 email = @email,
							 website = @website,
							 date_of_birth = @dateOfBirth,
							 company_name = @companyName,
							 position = @position,
							 work_email = @workEmail,
							 address_1 = @address1,
							 address_2 = @address2,
							 city = @city,
							 state_id = @stateId,
							 postal_code = @postalCode,
							 country_id = @countryId,
							 home_phone = @homePhone,
							 mobile_phone = @mobilePhone,
							 work_phone = @workPhone,
							 hashed_password = @hashedPassword,
							 security_stamp = @securityStamp,
							 enable_github_auth = @enableGithubAuth,
							 enable_google_auth = @enableGoogleAuth,
							 enable_microsoft_auth = @enableMicrosoftAuth,
							 enable_twitter_auth = @enableTwitterAuth
					   WHERE username = @username",
					new {
						firstName = updatedUser.FirstName,
						lastName = updatedUser.LastName,
						email = updatedUser.Email?.ToLower(),
						website = updatedUser.Website,
						dateOfBirth = updatedUser.DateOfBirth,
						companyName = updatedUser.Company,
						position = updatedUser.Position,
						workEmail = updatedUser.WorkEmail?.ToLower(),
						address1 = updatedUser.Address1,
						address2 = updatedUser.Address2,
						city = updatedUser.City,
						stateId = state?.Id,
						postalCode = updatedUser.PostalCode,
						countryId = country?.Id,
						homePhone = updatedUser.HomePhone,
						mobilePhone = updatedUser.MobilePhone,
						workPhone = updatedUser.WorkPhone,
						hashedPassword = updatedUser.HashedPassword,
						securityStamp = updatedUser.SecurityStamp,
						enableGithubAuth = updatedUser.IsGithubAuthEnabled,
						enableGoogleAuth = updatedUser.IsGoogleAuthEnabled,
						enableMicrosoftAuth = updatedUser.IsMicrosoftAuthEnabled,
						enableTwitterAuth = updatedUser.IsTwitterAuthEnabled,
						username = updatedUser.Username.ToLower()
					}
				).ConfigureAwait(false);

				if(result != 1)
				{
					throw new InvalidOperationException("Was not able to find user to update with username: " + updatedUser.Username.ToLower());
				}

				return true;
			});
        }


        public Task<bool> AddLoginHistoryAsync(string username, short loginActivityTypeId, short loginAreaId)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			return RunAsync(async conn => {
				var userId = await GetUserIdAsync(conn, "username", username.ToLower()).ConfigureAwait(false);

				return await AddLoginHistoryAsync(conn, userId, username.ToLower(), loginActivityTypeId, loginAreaId).ConfigureAwait(false);
			});
        }


		public Task<bool> AddExternalLoginHistoryAsync(string email, short loginActivityTypeId, short loginAreaId)
		{
			if(string.IsNullOrEmpty(email))
			{
				throw new ArgumentException("email must not be null or empty", nameof(email));
			}

			return RunAsync(async conn => {
				var userId = await GetUserIdAsync(conn, "email", email.ToLower()).ConfigureAwait(false);

				return await AddLoginHistoryAsync(conn, userId, email, loginActivityTypeId, loginAreaId).ConfigureAwait(false);
			});
		}


        public Task<IEnumerable<UserAndLastLogin>> GetUsersToManageAsync()
        {
			return RunAsync(conn => {
				return conn.QueryAsync<UserAndLastLogin>(
					@"SELECT id,
					         username,
							 first_name,
							 last_name,
							 (SELECT MAX(attempt_time)
							    FROM maw.login_history lh
							   WHERE lh.user_id = u.id
							 )
					    FROM maw.user u
					   ORDER BY u.username;"
				);
			});
        }


        public Task<IEnumerable<string>> GetAllUsernamesAsync()
        {
			return RunAsync(conn => {
				return conn.QueryAsync<string>(
					@"SELECT username
					    FROM maw.user
					   ORDER BY username;"
				);
			});
        }


        public Task<IEnumerable<string>> GetAllRoleNamesAsync()
        {
			return RunAsync(conn => {
				return conn.QueryAsync<string>(
					@"SELECT name
					    FROM maw.role
					   ORDER BY name;"
				);
			});
        }


        public Task<int> AddUserAsync(MawUser user)
        {
			if(user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"INSERT INTO maw.user
				           (
				    	     username,
				    		 hashed_password,
				    		 first_name,
				    		 last_name,
				    		 email,
				    		 security_stamp,
							 password_last_set_on
				    	   )
				      VALUES
				           (
				    		 @username,
				    		 @hashedPassword,
				    		 @firstName,
				    		 @lastName,
				    		 @email,
				    		 @securityStamp,
							 @passwordLastSetOn
				    	   );",
				    new {
				    	username = user.Username.ToLower(),
				    	hashedPassword = user.HashedPassword,
				    	firstName = user.FirstName,
				    	lastName = user.LastName,
				    	email = user.Email?.ToLower(),
				    	securityStamp = user.SecurityStamp,
						passwordLastSetOn = DateTime.Now
				    }
				).ConfigureAwait(false);

				user.Id = (short) await GetUserIdAsync(conn, "username", user.Username.ToLower()).ConfigureAwait(false);

				return result;
			});
        }


        public Task<int> RemoveUserAsync(string username)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			return RunAsync(async conn => {
				username = username.ToLower();

				await conn.ExecuteAsync(
					@"DELETE FROM maw.user_role
					   WHERE user_id = (SELECT id
					                      FROM maw.user
										 WHERE username = @username
									   );",
					new { username = username }
				);

				var result = await conn.ExecuteAsync(
					@"DELETE FROM maw.user
					   WHERE username = @username",
					new { username = username }
				);

				return result;
			});
        }


        public Task<IEnumerable<MawUser>> GetUsersInRoleAsync(string roleName)
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			return RunAsync(async conn => {
				var users = await conn.QueryAsync<MawUser, State, Country, MawUser>(
					$@"SELECT u.*,
						      u.company_name AS company,
							  u.enable_github_auth AS is_github_auth_enabled,
							  u.enable_google_auth AS is_google_auth_enabled,
							  u.enable_microsoft_auth AS is_microsoft_auth_enabled,
							  u.enable_twitter_auth AS is_twitter_auth_enabled,
							  s.*,
							  c.*
						 FROM maw.user u
						 LEFT OUTER JOIN maw.state s ON s.id = u.state_id
						 LEFT OUTER JOIN maw.country c ON c.id = u.country_id
					    WHERE u.id IN (SELECT user_id
						                 FROM maw.user_role
								  	    WHERE role_id = (SELECT id
									                       FROM maw.role
										                  WHERE name = @role
													  )
									);",
					(user, state, country) => {
							user.State = state?.Code;
							user.Country = country?.Code;
							return user;
					},
					new { role = roleName.ToLower() }
				).ConfigureAwait(false);

				foreach(var user in users)
				{
					await AddRolesForUser(conn, user).ConfigureAwait(false);
				}

				return users;
			});
        }


        public Task<bool> CreateRoleAsync(string roleName, string description)
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"INSERT INTO maw.role
					       (
							 name,
							 description
						   )
					  VALUES
					       (
							 @name,
							 @description
						   );",
					new {
						name = roleName.ToLower(),
						description = description
					}
				).ConfigureAwait(false);

				return result == 1;
			});
        }


        public Task<bool> RemoveRoleAsync(string roleName)
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			return RunAsync(async conn => {
				roleName = roleName.ToLower();

				var result = await conn.ExecuteAsync(
					@"DELETE FROM maw.user_role
					   WHERE role_id = (SELECT id
					                      FROM maw.role
										 WHERE name = @name
									   );",
					new { @name = roleName }
				).ConfigureAwait(false);

				result = await conn.ExecuteAsync(
					@"DELETE FROM maw.role
					   WHERE id = (SELECT id
					                 FROM maw.role
									WHERE name = @name
								  );",
					new { @name = roleName }
				).ConfigureAwait(false);

				return result == 1;
			});
        }


        public Task<bool> AddUserToRoleAsync(string username, string roleName)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"INSERT INTO maw.user_role
					       (
							 user_id,
							 role_id
						   )
					  VALUES
					       (
							 (SELECT id FROM maw.user WHERE username = @username),
							 (SELECT id FROM maw.role WHERE name = @role)
						   );",
					new {
						username = username.ToLower(),
						role = roleName.ToLower()
					}
				).ConfigureAwait(false);

				return result == 1;
			});
        }


        public Task<bool> RemoveUserFromRoleAsync(string username, string roleName)
        {
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"DELETE FROM maw.user_role
					   WHERE user_id = (SELECT id FROM maw.user WHERE username = @username)
					     AND role_id = (SELECT id FROM maw.role WHERE name = @role);",
					new {
						username = username.ToLower(),
						role = roleName.ToLower()
					}
				);

				return result == 1;
			});
        }


		public Task<bool> SetSecurityStampAsync(string username, string securityStamp)
		{
			if(string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("username must not be null or empty", nameof(username));
			}

			return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"UPDATE maw.user
					     SET security_stamp = @securityStamp
					   WHERE username = @username;",
					new {
						securityStamp = securityStamp,
						username = username.ToLower()
					}
				);

				return result == 1;
			});
		}


		public Task<MawRole> GetRoleAsync(string roleName)
		{
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
			}

			return GetRoleAsync("name", roleName.ToLower());
		}


		public Task<MawRole> GetRoleAsync(short id)
		{
			return GetRoleAsync("id", id);
		}


		Task<short?> GetUserIdAsync<T>(IDbConnection conn, string whereField, T whereValue)
		{
			return conn.QuerySingleOrDefaultAsync<short?>(
				$@"SELECT id
				     FROM maw.user u
					WHERE u.{whereField} = @whereValue;",
				new { whereValue = whereValue }
			);
		}


		Task<MawUser> GetUserAsync<T>(string whereField, T whereValue)
		{
			return RunAsync(async conn => {
				var userResult = await conn.QueryAsync<MawUser, State, Country, MawUser>(
						$@"SELECT u.*,
						          u.company_name AS company,
								  u.enable_github_auth AS is_github_auth_enabled,
								  u.enable_google_auth AS is_google_auth_enabled,
								  u.enable_microsoft_auth AS is_microsoft_auth_enabled,
								  u.enable_twitter_auth AS is_twitter_auth_enabled,
								  s.*,
								  c.*
							FROM maw.user u
							LEFT OUTER JOIN maw.state s ON s.id = u.state_id
							LEFT OUTER JOIN maw.country c ON c.id = u.country_id
						WHERE u.{whereField} = @whereValue;",
						(user, state, country) => {
							user.State = state?.Code;
							user.Country = country?.Code;
							return user;
						},
						new { whereValue = whereValue }
					).ConfigureAwait(false);

				var mawUser = userResult.FirstOrDefault();

				if(mawUser != null)
				{
					await AddRolesForUser(conn, mawUser);
				}

				return mawUser;
			});
		}


		Task<MawRole> GetRoleAsync<T>(string whereField, T whereValue)
		{
			return RunAsync(conn => {
				return conn.QuerySingleOrDefaultAsync<MawRole>(
					$@"SELECT *
					    FROM maw.role
					   WHERE {whereField} = @whereValue;",
					new { whereValue = whereValue }
				);
			});
		}


		async Task AddRolesForUser(IDbConnection conn, MawUser user)
		{
			var rolesResult = await conn.QueryAsync<MawRole>(
					@"SELECT *
						FROM maw.role r
					INNER JOIN maw.user_role ur ON ur.role_id = r.id
					WHERE ur.user_id = @userId",
					new { userId = user.Id }
				).ConfigureAwait(false);

			foreach(var role in rolesResult)
			{
				user.AddRole(role.Name);
			}
		}


		Task<State> GetStateAsync(IDbConnection conn, string code)
		{
			return conn.QuerySingleOrDefaultAsync<State>(
				@"SELECT *
				    FROM maw.state
				   WHERE code = @code;",
				new { code = code.ToUpper() }
			);
		}


		Task<Country> GetCountryAsync(IDbConnection conn, string code)
		{
			return conn.QuerySingleOrDefaultAsync<Country>(
				@"SELECT *
				    FROM maw.country
				   WHERE code = @code;",
				new { code = code.ToUpper() }
			);
		}


		async Task<bool> AddLoginHistoryAsync(IDbConnection conn, short? userId, string usernameOrEmail, short loginActivityTypeId, short loginAreaId)
		{
			var result = await conn.ExecuteAsync(
				@"INSERT INTO maw.login_history
				       (
						 user_id,
						 username,
						 login_activity_type_id,
						 login_area_id,
						 attempt_time
					   )
				  VALUES
				       (
						 @userId,
						 @username,
						 @loginActivityTypeId,
						 @loginAreaId,
						 @attemptTime
					   );",
				new {
					userId = userId,
					username = usernameOrEmail,
					loginActivityTypeId = loginActivityTypeId,
					loginAreaId = loginAreaId,
					attemptTime = DateTime.Now
				}
			).ConfigureAwait(false);

			return result == 1;
		}
    }
}
