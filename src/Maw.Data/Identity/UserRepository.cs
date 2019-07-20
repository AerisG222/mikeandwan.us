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
        public UserRepository(string connectionString)
            : base(connectionString)
        {

        }


        public Task<IEnumerable<State>> GetStatesAsync()
        {
            return RunAsync(conn =>
                conn.QueryAsync<State>(
                    "SELECT * FROM maw.get_states();"
                )
            );
        }


        public Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return RunAsync(conn =>
                conn.QueryAsync<Country>(
                    "SELECT * FROM maw.get_countries();"
                )
            );
        }


        public Task<MawUser> GetUserAsync(string username)
        {
            if(string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            return InternalGetUserAsync(username: username.ToLowerInvariant());
        }


        public Task<MawUser> GetUserAsync(short id)
        {
            return InternalGetUserAsync(id: id);
        }


        public Task<MawUser> GetUserByEmailAsync(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("email must not be null or empty", nameof(email));
            }

            return InternalGetUserAsync(email: email.ToLowerInvariant());
        }


        public async Task<IEnumerable<string>> GetRoleNamesForUserAsync(string username)
        {
            if(string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username must not be null or empty", nameof(username));
            }

            var roles = await InternalGetRolesForUserAsync(username).ConfigureAwait(false);

            return roles.Select(r => r.Name);
        }


        public Task<bool> UpdateUserPasswordAsync(MawUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return RunAsync(async conn => {
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.save_password(@username, @hashedPassword);",
                    new {
                        username = user.Username.ToLowerInvariant(),
                        hashedPassword = user.HashedPassword
                    }
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
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.update_user("
                        + " @username, @firstName, @lastName, @email, @hashedPassword, @securityStamp,"
                        + " @enableGithubAuth, @enableGoogleAuth, @enableMicrosoftAuth, @enableTwitterAuth);",
                    new {
                        username = updatedUser.Username.ToLowerInvariant(),
                        firstName = updatedUser.FirstName,
                        lastName = updatedUser.LastName,
                        email = updatedUser.Email?.ToLowerInvariant(),
                        hashedPassword = updatedUser.HashedPassword,
                        securityStamp = updatedUser.SecurityStamp,
                        enableGithubAuth = updatedUser.IsGithubAuthEnabled,
                        enableGoogleAuth = updatedUser.IsGoogleAuthEnabled,
                        enableMicrosoftAuth = updatedUser.IsMicrosoftAuthEnabled,
                        enableTwitterAuth = updatedUser.IsTwitterAuthEnabled
                    }
                ).ConfigureAwait(false);

                if(result != 1)
                {
                    throw new InvalidOperationException("Was not able to find user to update with username: " + updatedUser.Username.ToLowerInvariant());
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

            return RunAsync(conn =>
                AddLoginHistoryAsync(conn, username.ToLowerInvariant(), null, loginActivityTypeId, loginAreaId)
            );
        }


        public Task<bool> AddExternalLoginHistoryAsync(string email, short loginActivityTypeId, short loginAreaId)
        {
            if(string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("email must not be null or empty", nameof(email));
            }

            return RunAsync(conn =>
                AddLoginHistoryAsync(conn, null, email, loginActivityTypeId, loginAreaId)
            );
        }


        public Task<IEnumerable<UserAndLastLogin>> GetUsersToManageAsync()
        {
            return RunAsync(conn =>
                conn.QueryAsync<UserAndLastLogin>(
                    "SELECT * FROM maw.get_user_detail();"
                )
            );
        }


        public Task<IEnumerable<string>> GetAllUsernamesAsync()
        {
            return RunAsync(conn =>
                conn.QueryAsync<string>(
                    "SELECT * FROM maw.get_usernames();"
                )
            );
        }


        public Task<IEnumerable<string>> GetAllRoleNamesAsync()
        {
            return RunAsync(conn =>
                conn.QueryAsync<string>(
                    "SELECT * FROM maw.get_role_names();"
                )
            );
        }


        public Task<short> AddUserAsync(MawUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return RunAsync(conn =>
                conn.QuerySingleAsync<short>(
                    "SELECT * FROM maw.add_user(@username, @hashedPassword, @firstName,"
                                             + "@lastName, @email, @securityStamp, @passwordLastSetOn);",
                    new {
                        username = user.Username.ToLowerInvariant(),
                        hashedPassword = user.HashedPassword,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email?.ToLowerInvariant(),
                        securityStamp = user.SecurityStamp,
                        passwordLastSetOn = DateTime.Now
                    }
                )
            );
        }


        public Task<long> RemoveUserAsync(string username)
        {
            if(string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username must not be null or empty", nameof(username));
            }

            return RunAsync(conn =>
                conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.remove_user(@username);",
                    new { username = username.ToLowerInvariant() }
                )
            );
        }


        public Task<IEnumerable<MawUser>> GetUsersInRoleAsync(string roleName)
        {
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
            }

            return RunAsync(async conn => {
                var users = await conn.QueryAsync<MawUser>(
                    "SELECT * FROM maw.get_users_in_role(@roleName);",
                    new { roleName = roleName.ToLowerInvariant() }
                ).ConfigureAwait(false);

                foreach(var user in users)
                {
                    await AddRolesForUser(user).ConfigureAwait(false);
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
                var result = await conn.QuerySingleAsync<short>(
                    "SELECT * FROM maw.add_role(@name, @description);",
                    new {
                        name = roleName.ToLowerInvariant(),
                        description
                    }
                ).ConfigureAwait(false);

                return result > 0;
            });
        }


        public Task<bool> RemoveRoleAsync(string roleName)
        {
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
            }

            return RunAsync(async conn => {
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.remove_role(@name);",
                    new { name = roleName.ToLowerInvariant() }
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
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.add_user_to_role(@username, @role);",
                    new {
                        username = username.ToLowerInvariant(),
                        role = roleName.ToLowerInvariant()
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
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.remove_user_from_role(@username, @role);",
                    new {
                        username = username.ToLowerInvariant(),
                        role = roleName.ToLowerInvariant()
                    }
                ).ConfigureAwait(false);

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
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.save_security_stamp(@username, @securityStamp);",
                    new {
                        username = username.ToLowerInvariant(),
                        securityStamp
                    }
                ).ConfigureAwait(false);

                return result == 1;
            });
        }


        public Task<MawRole> GetRoleAsync(string roleName)
        {
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("roleName must not be null or empty", nameof(roleName));
            }

            return InternalGetRoleAsync(name: roleName.ToLowerInvariant());
        }


        public Task<MawRole> GetRoleAsync(short id)
        {
            return InternalGetRoleAsync(id: id);
        }


        Task<MawUser> InternalGetUserAsync(short? id = null, string username = null, string email = null)
        {
            return RunAsync(async conn => {
                var mawUser = await conn.QuerySingleOrDefaultAsync<MawUser>(
                        "SELECT * FROM maw.get_users(@id, @username, @email);",
                        new {
                            id,
                            username,
                            email
                        }
                    ).ConfigureAwait(false);

                if(mawUser != null)
                {
                    await AddRolesForUser(mawUser).ConfigureAwait(false);
                }

                return mawUser;
            });
        }


        Task<MawRole> InternalGetRoleAsync(short? id = null, string name = null)
        {
            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<MawRole>(
                    "SELECT * FROM maw.get_roles(@id, @name);",
                    new {
                        id,
                        name
                    }
                )
            );
        }


        Task<IEnumerable<MawRole>> InternalGetRolesForUserAsync(string username)
        {
            return RunAsync(conn =>
                conn.QueryAsync<MawRole>(
                    "SELECT * FROM maw.get_roles_for_user(@username);",
                    new { username = username.ToLowerInvariant() }
                )
            );
        }


        async Task AddRolesForUser(MawUser user)
        {
            var rolesResult = await InternalGetRolesForUserAsync(user.Username).ConfigureAwait(false);

            foreach(var role in rolesResult)
            {
                user.AddRole(role.Name);
            }
        }


        async Task<bool> AddLoginHistoryAsync(IDbConnection conn, string username, string email, short loginActivityTypeId, short loginAreaId)
        {
            var result = await conn.QuerySingleAsync<long>(
                "SELECT * FROM maw.add_login_history(@loginActivityTypeId, @loginAreaId, @attemptTime, @username, @email);",
                new {
                    loginActivityTypeId,
                    loginAreaId,
                    attemptTime = DateTime.Now,
                    username,
                    email
                }
            ).ConfigureAwait(false);

            return result == 1;
        }
    }
}
