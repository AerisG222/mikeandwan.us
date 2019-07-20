using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.Stores;


namespace MawAuth.Services
{
    public class MawSigningCredentialStore
        : ISigningCredentialStore
    {
        readonly SigningCredentials _creds;


#pragma warning disable CA2000
        public MawSigningCredentialStore(string signingCertificateDirectory)
        {
            if(string.IsNullOrWhiteSpace(signingCertificateDirectory))
            {
                throw new ArgumentNullException(nameof(signingCertificateDirectory));
            }

            var certFile = Path.Combine(signingCertificateDirectory, "signing.pfx");
            var pwdFile = Path.Combine(signingCertificateDirectory, "signing.pfx.pwd");
            var pwd = File.ReadAllText(pwdFile).Trim();
            var cert = new X509Certificate2(certFile, pwd);

            _creds = new SigningCredentials(new X509SecurityKey(cert), "RS256");
        }
#pragma warning restore CA2000

        public Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            return Task.FromResult(_creds);
        }
    }
}
