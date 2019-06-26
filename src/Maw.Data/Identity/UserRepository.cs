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

            return InternalGetUserAsync(username: username.ToLower());
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

            return InternalGetUserAsync(email: email.ToLower());
        }


        public async Task<IEnumerable<string>> GetRoleNamesForUserAsync(string username)
        {
            if(string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username must not be null or empty", nameof(username));
            }

            var roles = await InternalGetRolesForUserAsync(username);

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
                        username = user.Username.ToLower(),
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
                        username = updatedUser.Username.ToLower(),
                        firstName = updatedUser.FirstName,
                        lastName = updatedUser.LastName,
                        email = updatedUser.Email?.ToLower(),
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

            return RunAsync(conn =>
                AddLoginHistoryAsync(conn, username.ToLower(), null, loginActivityTypeId, loginAreaId)
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
                        username = user.Username.ToLower(),
                        hashedPassword = user.HashedPassword,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email?.ToLower(),
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
                    new { username = username.ToLower() }
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
                    new { roleName = roleName.ToLower() }
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
                var result = await conn.QuerySingleAsync<short>(
                    "SELECT * FROM maw.add_role(@name, @description);",
                    new {
                        name = roleName.ToLower(),
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
                    new { name = roleName.ToLower() }
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
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.remove_user_from_role(@username, @role);",
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
                var result = await conn.QuerySingleAsync<long>(
                    "SELECT * FROM maw.save_security_stamp(@username, @securityStamp);",
                    new {
                        username = username.ToLower(),
                        securityStamp
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

            return InternalGetRoleAsync(name: roleName.ToLower());
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
                    await AddRolesForUser(conn, mawUser);
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
                    new { username = username.ToLower() }
                )
            );
        }


        async Task AddRolesForUser(IDbConnection conn, MawUser user)
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
