﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Claims;
using IdentityModel;

namespace Maw.Domain.Models.Identity;

public class MawUser
    : ClaimsIdentity
{
    public short Id
    {
        get
        {
            var id = GetSingleClaim(MawClaimTypes.UserId);

            return id == null ? (short)0 : short.Parse(id, CultureInfo.InvariantCulture);
        }
        set
        {
            SetSingleClaim(MawClaimTypes.UserId, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    [DisallowNull]
    public string? Username
    {
        get => GetSingleClaim(JwtClaimTypes.Name);
        set => SetSingleClaim(JwtClaimTypes.Name, value);
    }

    [DisallowNull]
    public string? Email
    {
        get => GetSingleClaim(JwtClaimTypes.Email);
        set => SetSingleClaim(JwtClaimTypes.Email, value);
    }

    [DisallowNull]
    public string? HashedPassword
    {
        get => GetSingleClaim(ClaimTypes.Hash);
        set => SetSingleClaim(ClaimTypes.Hash, value);
    }

    [DisallowNull]
    public string? FirstName
    {
        get => GetSingleClaim(JwtClaimTypes.GivenName);
        set => SetSingleClaim(JwtClaimTypes.GivenName, value);
    }

    [DisallowNull]
    public string? LastName
    {
        get => GetSingleClaim(JwtClaimTypes.FamilyName);
        set => SetSingleClaim(JwtClaimTypes.FamilyName, value);
    }

    public string? SecurityStamp { get; set; }

    public bool IsGithubAuthEnabled
    {
        get => GetBooleanClaim(MawClaimTypes.EnableGithubAuth, false);
        set => SetSingleClaim(MawClaimTypes.EnableGithubAuth, value.ToString(CultureInfo.InvariantCulture));
    }

    public bool IsGoogleAuthEnabled
    {
        get => GetBooleanClaim(MawClaimTypes.EnableGoogleAuth, false);
        set => SetSingleClaim(MawClaimTypes.EnableGoogleAuth, value.ToString(CultureInfo.InvariantCulture));
    }

    public bool IsMicrosoftAuthEnabled
    {
        get => GetBooleanClaim(MawClaimTypes.EnableMicrosoftAuth, false);
        set => SetSingleClaim(MawClaimTypes.EnableMicrosoftAuth, value.ToString(CultureInfo.InvariantCulture));
    }

    public bool IsTwitterAuthEnabled
    {
        get => GetBooleanClaim(MawClaimTypes.EnableTwitterAuth, false);
        set => SetSingleClaim(MawClaimTypes.EnableTwitterAuth, value.ToString(CultureInfo.InvariantCulture));
    }

    public void AddRole(string role)
    {
        AddClaim(new Claim(JwtClaimTypes.Role, role));
    }

    public void RemoveRole(string role)
    {
        RemoveClaim(new Claim(JwtClaimTypes.Role, role));
    }

    public IEnumerable<string> GetRoles()
    {
        return FindAll(JwtClaimTypes.Role).Select(x => x.Value);
    }

    public static MawUser Convert(ClaimsIdentity identity)
    {
        if (identity == null)
        {
            throw new ArgumentNullException(nameof(identity));
        }

        var mawUser = new MawUser();

        mawUser.AddClaims(identity.Claims);

        return mawUser;
    }

    public bool IsExternalAuthEnabled([NotNullWhen(true)] string? provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return false;
        }

        return provider.ToUpperInvariant() switch
        {
            "GITHUB" => IsGithubAuthEnabled,
            "GOOGLE" => IsGoogleAuthEnabled,
            "MICROSOFT" => IsMicrosoftAuthEnabled,
            "TWITTER" => IsTwitterAuthEnabled,
            _ => false
        };
    }

    string? GetSingleClaim(string claimType)
    {
        var val = FindFirst(claimType);

        return val?.Value;
    }

    bool GetBooleanClaim(string claimType, bool defaultValue)
    {
        var val = GetSingleClaim(claimType);

        if(bool.TryParse(val, out var result))
        {
            return result;
        }

        return defaultValue;
    }

    void SetSingleClaim(string claimType, string value)
    {
        var claims = FindAll(claimType).ToArray();

        for (var i = 0; i < claims.Length; i++)
        {
            RemoveClaim(claims[i]);
        }

        if (value != null)
        {
            AddClaim(new Claim(claimType, value));
        }
    }
}
