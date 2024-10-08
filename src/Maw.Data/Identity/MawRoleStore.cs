﻿using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Maw.Data.Abstractions;
using Maw.Domain.Models.Identity;

namespace Maw.Data.Identity;

public class MawRoleStore
    : IRoleStore<MawRole>, IDisposable
{
    readonly IUserRepository _repo;

    #region ctor
    public MawRoleStore(
        IUserRepository repo)
    {
        ArgumentNullException.ThrowIfNull(repo);

        _repo = repo;
    }
    #endregion

    #region IRoleStore
    public async Task<IdentityResult> CreateAsync(MawRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(role.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(role.Description);

        var result = await _repo.CreateRoleAsync(role.Name, role.Description);

        return result ? IdentityResult.Success : IdentityResult.Failed();
    }

    public Task<IdentityResult> UpdateAsync(MawRole role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> DeleteAsync(MawRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(role.Name);

        var result = await _repo.RemoveRoleAsync(role.Name);

        return result ? IdentityResult.Success : IdentityResult.Failed();
    }

    public Task<string> GetRoleIdAsync(MawRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult(role.Id.ToString(CultureInfo.InvariantCulture));
    }

    public Task<string?> GetRoleNameAsync(MawRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult(role.Name);
    }

    public Task<string?> GetNormalizedRoleNameAsync(MawRole role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(MawRole role, string? normalizedName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        return Task.CompletedTask;
    }

    public Task SetRoleNameAsync(MawRole role, string? roleName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (string.IsNullOrEmpty(roleName))
        {
            throw new ArgumentException("roleName must not be null and have a value.", nameof(roleName));
        }

        role.Name = roleName;

        return Task.CompletedTask;
    }

    public Task<MawRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            throw new ArgumentException("Invalid roleId", nameof(roleId));
        }

        if (short.TryParse(roleId, out short id))
        {
            return _repo.GetRoleAsync(id);
        }

        throw new ArgumentException("roleId should be able to be parsed into a short.", nameof(roleId));
    }

    public Task<MawRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(normalizedRoleName))
        {
            throw new ArgumentException("Invalid roleName", nameof(normalizedRoleName));
        }

        return _repo.GetRoleAsync(normalizedRoleName);
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
