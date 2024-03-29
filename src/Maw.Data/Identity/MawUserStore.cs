﻿using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Maw.Data.Abstractions;
using Maw.Domain.Models.Identity;

namespace Maw.Data.Identity;

public class MawUserStore
    : IUserStore<MawUser>, IUserPasswordStore<MawUser>, IUserRoleStore<MawUser>, IUserSecurityStampStore<MawUser>, IUserEmailStore<MawUser>, IDisposable
{
    readonly IUserRepository _repo;
    readonly ILogger _log;

    public MawUserStore(
        IUserRepository repo,
        ILogger<MawUserStore> log)
    {
        ArgumentNullException.ThrowIfNull(repo);
        ArgumentNullException.ThrowIfNull(log);

        _repo = repo;
        _log = log;
    }

    #region IUserStore
    public virtual Task<string> GetUserIdAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        _log.LogInformation("getting userid: {UserId}", user.Id);

        return Task.FromResult(user.Id.ToString(CultureInfo.InvariantCulture));
    }

    public virtual Task<string?> GetUserNameAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.Username);
    }

    public virtual Task SetUserNameAsync(MawUser user, string? userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // we do not currently support changing usernames
        throw new NotImplementedException();
    }

    public virtual Task<string?> GetNormalizedUserNameAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.Username);
    }

    public virtual Task SetNormalizedUserNameAsync(MawUser user, string? normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // we do not currently support changing usernames, just exit
        return Task.CompletedTask;
    }

    public virtual async Task<IdentityResult> CreateAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        _log.LogInformation("creating new user: {Username}", user.Username);

        var result = await _repo.AddUserAsync(user);

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

        ArgumentNullException.ThrowIfNull(user);

        var success = await _repo.UpdateUserAsync(user);

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

        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrEmpty(user.Username))
        {
            throw new ArgumentException("Username cannot be null");
        }

        if (1 == await _repo.RemoveUserAsync(user.Username))
        {
            return IdentityResult.Success;
        }
        else
        {
            return IdentityResult.Failed();
        }
    }

    public virtual async Task<MawUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _log.LogDebug("attempting to find by user by ID: {UserId}", userId);

        if (short.TryParse(userId, out short id))
        {
            var user = await _repo.GetUserAsync(id);

            return user;
        }

        throw new ArgumentException($"{ nameof(userId) } should be a number", nameof(userId));
    }

    public virtual async Task<MawUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _log.LogInformation("attempting to find user by name: {Username}", normalizedUserName);

        return await _repo.GetUserAsync(normalizedUserName);
    }
    #endregion

    #region IUserPasswordStore
    public virtual Task SetPasswordHashAsync(MawUser user, string? passwordHash, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        user.HashedPassword = passwordHash;

        return Task.CompletedTask;
    }

    public virtual Task<string?> GetPasswordHashAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.HashedPassword);
    }

    public virtual Task<bool> HasPasswordAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        var hasHash = !string.IsNullOrEmpty(user.HashedPassword);

        return Task.FromResult(hasHash);
    }
    #endregion

    #region IUserRoleStore
    public virtual Task AddToRoleAsync(MawUser user, string roleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Username);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        return _repo.AddUserToRoleAsync(user.Username, roleName);
    }

    public virtual Task RemoveFromRoleAsync(MawUser user, string roleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Username);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        _log.LogInformation("removing user {Username} from role {Role}", user.Username, roleName);

        return _repo.RemoveUserFromRoleAsync(user.Username, roleName);
    }

    public virtual Task<IList<string>> GetRolesAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

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

        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        _log.LogInformation("is user {Username} in role: {Role}", user.Username, roleName);

        var roles = await GetRolesAsync(user, cancellationToken);
        var role = roles.SingleOrDefault(x => string.Equals(x, roleName, StringComparison.OrdinalIgnoreCase));

        return role != null;
    }

    public virtual async Task<IList<MawUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        _log.LogDebug("getting users in role: {Role}", roleName);

        return (await _repo.GetUsersInRoleAsync(roleName)).ToList();
    }
    #endregion

    #region ISecurityStampStore
    public virtual Task SetSecurityStampAsync(MawUser user, string stamp, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(stamp);

        user.SecurityStamp = stamp;

        return Task.CompletedTask;
    }

    public virtual Task<string?> GetSecurityStampAsync(MawUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.SecurityStamp);
    }
    #endregion

    #region IUserEmailStore
    public Task SetEmailAsync(MawUser user, string? email, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("email can not be null or empty", nameof(email));
        }

        user.Email = email;

        return _repo.UpdateUserAsync(user);
    }

    public Task<string?> GetEmailAsync(MawUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

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

    public Task<MawUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        _log.LogDebug("finding user by email: {Email}", normalizedEmail);

        return _repo.GetUserByEmailAsync(normalizedEmail);
    }

    public Task<string?> GetNormalizedEmailAsync(MawUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.Email);
    }

    public Task SetNormalizedEmailAsync(MawUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
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
