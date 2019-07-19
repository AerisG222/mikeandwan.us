using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using IdentityModel;


namespace Maw.Domain.Identity
{
    public class MawUser
		: ClaimsIdentity
    {
		public short Id
		{
			get
			{
				var id = GetSingleClaim(MawClaimTypes.UserId);

				return id == null ?(short) 0 : short.Parse(id, CultureInfo.InvariantCulture);
			}
			set
			{
				SetSingleClaim(MawClaimTypes.UserId, value.ToString(CultureInfo.InvariantCulture));
			}
		}


		public string Username
		{
			get { return GetSingleClaim(JwtClaimTypes.Name); }
			set { SetSingleClaim(JwtClaimTypes.Name, value); }
		}


		public string Email
		{
			get { return GetSingleClaim(JwtClaimTypes.Email); }
			set { SetSingleClaim(JwtClaimTypes.Email, value); }
		}


		public string HashedPassword
		{
			get { return GetSingleClaim(ClaimTypes.Hash); }
			set { SetSingleClaim(ClaimTypes.Hash, value); }
		}


        public string FirstName
        {
            get { return GetSingleClaim(JwtClaimTypes.GivenName); }
			set { SetSingleClaim(JwtClaimTypes.GivenName, value); }
        }


        public string LastName
        {
            get { return GetSingleClaim(JwtClaimTypes.FamilyName); }
			set { SetSingleClaim(JwtClaimTypes.FamilyName, value); }
        }


		public string SecurityStamp { get; set; }


        public bool IsGithubAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(MawClaimTypes.EnableGithubAuth)); }
            set { SetSingleClaim(MawClaimTypes.EnableGithubAuth, value.ToString(CultureInfo.InvariantCulture)); }
        }


        public bool IsGoogleAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(MawClaimTypes.EnableGoogleAuth)); }
            set { SetSingleClaim(MawClaimTypes.EnableGoogleAuth, value.ToString(CultureInfo.InvariantCulture)); }
        }


        public bool IsMicrosoftAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(MawClaimTypes.EnableMicrosoftAuth)); }
            set { SetSingleClaim(MawClaimTypes.EnableMicrosoftAuth, value.ToString(CultureInfo.InvariantCulture)); }
        }


        public bool IsTwitterAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(MawClaimTypes.EnableTwitterAuth)); }
            set { SetSingleClaim(MawClaimTypes.EnableTwitterAuth, value.ToString(CultureInfo.InvariantCulture)); }
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
            if(identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

			var mawUser = new MawUser();

			mawUser.AddClaims(identity.Claims);

			return mawUser;
		}


        public bool IsExternalAuthEnabled(string provider)
        {
            if(string.IsNullOrWhiteSpace(provider))
            {
                return false;
            }

            switch(provider.ToUpperInvariant())
            {
                case "GITHUB":
                    return IsGithubAuthEnabled;
                case "GOOGLE":
                    return IsGoogleAuthEnabled;
                case "MICROSOFT":
                    return IsMicrosoftAuthEnabled;
                case "TWITTER":
                    return IsTwitterAuthEnabled;
                default:
                    return false;
            }
        }


        string GetSingleClaim(string claimType)
        {
			var val = FindFirst(claimType);

			return val == null ? null : val.Value;
        }


        void SetSingleClaim(string claimType, string value)
        {
			var claims = FindAll(claimType).ToArray();

			for(var i = 0; i < claims.Count(); i++)
			{
				RemoveClaim(claims[i]);
			}

            if(value != null)
			{
				AddClaim(new Claim(claimType, value));
			}
        }
    }
}
