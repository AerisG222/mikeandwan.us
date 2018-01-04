using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace Maw.Domain.Identity
{
    public class MawUser
		: ClaimsIdentity
    {
        const string CLAIM_TYPE_COMPANY = "http://mikeandwan.us/claimTypes/company";
        const string CLAIM_TYPE_POSITION = "http://mikeandwan.us/claimTypes/position";
        const string CLAIM_TYPE_WORK_EMAIL = "http://mikeandwan.us/claimTypes/workemail";
        const string CLAIM_TYPE_ADDRESS2 = "http://mikeandwan.us/claimTypes/address2";
		const string CLAIM_TYPE_USERID = "http://mikeandwan.us/claimTypes/userid";
		const string CLAIM_TYPE_SECURITY_STAMP = "http://mikeandwan.us/claimTypes/security_stamp";
        const string CLAIM_TYPE_ENABLE_GITHUB_AUTH = "http://mikeandwan.us/claimTypes/enable_github_auth";
        const string CLAIM_TYPE_ENABLE_GOOGLE_AUTH = "http://mikeandwan.us/claimTypes/enable_google_auth";
        const string CLAIM_TYPE_ENABLE_MICROSOFT_AUTH = "http://mikeandwan.us/claimTypes/enable_microsoft_auth";
        const string CLAIM_TYPE_ENABLE_TWITTER_AUTH = "http://mikeandwan.us/claimTypes/enable_twitter_auth";


		public short Id
		{
			get
			{
				var id = GetSingleClaim(CLAIM_TYPE_USERID);

				return id == null ?(short) 0 : short.Parse(id);
			}
			set
			{
				SetSingleClaim(CLAIM_TYPE_USERID, value.ToString());
			}
		}


		public string Username
		{
			get { return GetSingleClaim(ClaimTypes.Name); }
			set { SetSingleClaim(ClaimTypes.Name, value); }
		}


		public string Email
		{
			get { return GetSingleClaim("email"); }
			set { SetSingleClaim("email", value); }
		}


		public string HashedPassword
		{
			get { return GetSingleClaim(ClaimTypes.Hash); }
			set { SetSingleClaim(ClaimTypes.Hash, value); }
		}


        public string FirstName
        {
            get { return GetSingleClaim("given_name"); }
			set { SetSingleClaim("given_name", value); }
        }


        public string LastName
        {
            get { return GetSingleClaim("family_name"); }
			set { SetSingleClaim("family_name", value); }
        }


        public DateTime? DateOfBirth
        {
            get
            {
                var dob = GetSingleClaim(ClaimTypes.DateOfBirth);

				return string.IsNullOrEmpty(dob) ? (DateTime?)null : DateTime.Parse(dob);
            }
            set { SetSingleClaim(ClaimTypes.DateOfBirth, value.ToString()); }
        }


        public string Company
        {
            get { return GetSingleClaim(CLAIM_TYPE_COMPANY); }
            set { SetSingleClaim(CLAIM_TYPE_COMPANY, value); }
        }


        public string Position
        {
            get { return GetSingleClaim(CLAIM_TYPE_POSITION); }
            set { SetSingleClaim(CLAIM_TYPE_POSITION, value); }
        }


        public string WorkEmail
        {
            get { return GetSingleClaim(CLAIM_TYPE_WORK_EMAIL); }
            set { SetSingleClaim(CLAIM_TYPE_WORK_EMAIL, value); }
        }


		public string HomePhone
		{
			get { return GetSingleClaim(ClaimTypes.HomePhone); }
			set { SetSingleClaim(ClaimTypes.HomePhone, value); }
		}


        public string MobilePhone
        {
            get { return GetSingleClaim(ClaimTypes.MobilePhone); }
            set { SetSingleClaim(ClaimTypes.MobilePhone, value); }
        }


        public string WorkPhone
        {
            get { return GetSingleClaim(ClaimTypes.OtherPhone); }
            set { SetSingleClaim(ClaimTypes.OtherPhone, value); }
        }


        public string Address1
        {
            get { return GetSingleClaim(ClaimTypes.StreetAddress); }
            set { SetSingleClaim(ClaimTypes.StreetAddress, value); }
        }


        public string Address2
        {
            get { return GetSingleClaim(CLAIM_TYPE_ADDRESS2); }
            set { SetSingleClaim(CLAIM_TYPE_ADDRESS2, value); }
        }


        public string City
        {
            get { return GetSingleClaim(ClaimTypes.Locality); }
            set { SetSingleClaim(ClaimTypes.Locality, value); }
        }


        public string State
        {
            get { return GetSingleClaim(ClaimTypes.StateOrProvince); }
            set { SetSingleClaim(ClaimTypes.StateOrProvince, value); }
        }


        public string PostalCode
        {
            get { return GetSingleClaim(ClaimTypes.PostalCode); }
            set { SetSingleClaim(ClaimTypes.PostalCode, value); }
        }


        public string Country
        {
            get { return GetSingleClaim(ClaimTypes.Country); }
            set { SetSingleClaim(ClaimTypes.Country, value); }
        }


        public string Website
        {
            get { return GetSingleClaim(ClaimTypes.Webpage); }
            set { SetSingleClaim(ClaimTypes.Webpage, value); }
        }


		public string SecurityStamp
		{
			get { return GetSingleClaim(CLAIM_TYPE_SECURITY_STAMP); }
			set { SetSingleClaim(CLAIM_TYPE_SECURITY_STAMP, value); }
		}


        public bool IsGithubAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(CLAIM_TYPE_ENABLE_GITHUB_AUTH)); }
            set { SetSingleClaim(CLAIM_TYPE_ENABLE_GITHUB_AUTH, value.ToString()); }
        }


        public bool IsGoogleAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(CLAIM_TYPE_ENABLE_GOOGLE_AUTH)); }
            set { SetSingleClaim(CLAIM_TYPE_ENABLE_GOOGLE_AUTH, value.ToString()); }
        }


        public bool IsMicrosoftAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(CLAIM_TYPE_ENABLE_MICROSOFT_AUTH)); }
            set { SetSingleClaim(CLAIM_TYPE_ENABLE_MICROSOFT_AUTH, value.ToString()); }
        }


        public bool IsTwitterAuthEnabled
        {
            get { return bool.Parse(GetSingleClaim(CLAIM_TYPE_ENABLE_TWITTER_AUTH)); }
            set { SetSingleClaim(CLAIM_TYPE_ENABLE_TWITTER_AUTH, value.ToString()); }
        }


		public void AddRole(string role)
		{
			AddClaim(new Claim(ClaimTypes.Role, role));
		}


		public void RemoveRole(string role)
		{
			RemoveClaim(new Claim(ClaimTypes.Role, role));
		}


		public IEnumerable<string> GetRoles()
		{
			return FindAll(ClaimTypes.Role).Select(x => x.Value);
		}


		public static MawUser Convert(ClaimsIdentity identity)
		{
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

            provider = provider.ToLower();

            switch(provider)
            {
                case "github":
                    return IsGithubAuthEnabled;
                case "google":
                    return IsGoogleAuthEnabled;
                case "microsoft":
                    return IsMicrosoftAuthEnabled;
                case "twitter":
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