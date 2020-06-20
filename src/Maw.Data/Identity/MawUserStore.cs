using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;


namespace Maw.Data.Identity
{
    public class MawUserStore
        : IUserStore<MawUser>, IUserPasswordStore<MawUser>, IUserRoleStore<MawUser>, IUserSecurityStampStore<MawUser>, IUserEmailStore<MawUser>, IDisposable
    {
        readonly IUserRepository _repo;
        readonly ILogger _log;


        #region ctor
        public MawUserStore(IUserRepository repo, ILogger<MawUserStore> log)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        #endregion


        #region IUserStore
        public virtual Task<string> GetUserIdAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _log.LogInformation("getting userid: {UserId}", user.Id);

            return Task.FromResult(user.Id.ToString(CultureInfo.InvariantCulture));
        }


        public virtual Task<string> GetUserNameAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Username);
        }


        public virtual Task SetUserNameAsync(MawUser user, string userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // we do not currently support changing usernames
            throw new NotImplementedException();
        }


        public virtual Task<string> GetNormalizedUserNameAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Username);
        }


        public virtual Task SetNormalizedUserNameAsync(MawUser user, string userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // we do not currently support changing usernames, just exit
            return Task.FromResult(0);
        }


        public virtual async Task<IdentityResult> CreateAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _log.LogInformation("creating new user: {Username}", user.Username);

            cancellationToken.ThrowIfCancellationRequested();

            var result = await _repo.AddUserAsync(user).ConfigureAwait(false);

            if (result > 0)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }


        public virtual async Task<IdentityResult> UpdateAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var success = await _repo.UpdateUserAsync(user).ConfigureAwait(false);

            if (success)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }


        public virtual async Task<IdentityResult> DeleteAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (1 == await _repo.RemoveUserAsync(user.Username).ConfigureAwait(false))
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }


        public virtual async Task<MawUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _log.LogDebug("attempting to find by user by ID: {UserId}", userId);

            if (short.TryParse(userId, out short id))
            {
                var user = await _repo.GetUserAsync(id).ConfigureAwait(false);

                if (user == null)
                {
                    throw new Exception("User was not found");
                }

                return user;
            }

            throw new ArgumentException("userId should be a number", nameof(userId));
        }


        public virtual async Task<MawUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _log.LogInformation("attempting to find user by name: {Username}", normalizedUserName);

            var user = await _repo.GetUserAsync(normalizedUserName).ConfigureAwait(false);

            return user;
        }
        #endregion


        #region IUserPasswordStore
        public virtual Task SetPasswordHashAsync(MawUser user, string passwordHash, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.HashedPassword = passwordHash;

            return Task.FromResult(0);
        }


        public virtual Task<string> GetPasswordHashAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.HashedPassword);
        }


        public virtual Task<bool> HasPasswordAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var hasHash = !string.IsNullOrEmpty(user.HashedPassword);

            return Task.FromResult(hasHash);
        }
        #endregion


        #region IUserRoleStore
        public virtual Task AddToRoleAsync(MawUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("roleName cannot be null or empty", nameof(roleName));
            }

            return _repo.AddUserToRoleAsync(user.Username, roleName);
        }


        public virtual Task RemoveFromRoleAsync(MawUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("roleName cannot be null or empty", nameof(roleName));
            }

            _log.LogInformation("removing user {Username} from role {Role}", user.Username, roleName);

            return _repo.RemoveUserFromRoleAsync(user.Username, roleName);
        }


        public virtual Task<IList<string>> GetRolesAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _log.LogDebug("getting roles for: {Username}", user.Username);

            IList<string> list = user.GetRoles().ToList();

            foreach (var r in list)
            {
                _log.LogDebug("    in role: {Role}", r);
            }

            return Task.FromResult(list);
        }


        public virtual async Task<bool> IsInRoleAsync(MawUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("roleName cannot be null or empty", nameof(roleName));
            }

            _log.LogInformation("is user in role: " + user.Username + " : " + roleName);

            var roles = await GetRolesAsync(user, cancellationToken).ConfigureAwait(false);
            var role = roles.SingleOrDefault(x => string.Equals(x, roleName, StringComparison.OrdinalIgnoreCase));

            return role != null;
        }


        public virtual async Task<IList<MawUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("roleName can not be null or empty", nameof(roleName));
            }

            _log.LogDebug("getting users in role: {Role}", roleName);

            return (await _repo.GetUsersInRoleAsync(roleName).ConfigureAwait(false)).ToList();
        }
        #endregion


        #region ISecurityStampStore
        public virtual Task SetSecurityStampAsync(MawUser user, string stamp, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(stamp))
            {
                throw new ArgumentException("stamp can not be null or empty", nameof(stamp));
            }

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }


        public virtual Task<string> GetSecurityStampAsync(MawUser user, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }
        #endregion


        #region IUserEmailStore
        public Task SetEmailAsync(MawUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = email;

            return _repo.UpdateUserAsync(user);
        }


        public Task<string> GetEmailAsync(MawUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }


        public Task<bool> GetEmailConfirmedAsync(MawUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }


        public Task SetEmailConfirmedAsync(MawUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task<MawUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            _log.LogDebug("finding user by email: {Email}", normalizedEmail);

            return _repo.GetUserByEmailAsync(normalizedEmail);
        }


        public Task<string> GetNormalizedEmailAsync(MawUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }


        public Task SetNormalizedEmailAsync(MawUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
        #endregion


        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // clean up managed resources
            }
        }
        #endregion
    }
}
