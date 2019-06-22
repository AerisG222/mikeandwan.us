using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.Stores;


namespace MawAuth.Services
{
    public class MawValidationKeysStore
        : IValidationKeysStore
    {
        readonly List<SecurityKey> _keys = new List<SecurityKey>();


        public MawValidationKeysStore(string signingCertificateDirectory)
        {
            LoadKeys(signingCertificateDirectory);
        }


        public Task<IEnumerable<SecurityKey>> GetValidationKeysAsync()
        {
            return Task.FromResult((IEnumerable<SecurityKey>)_keys);
        }


        void LoadKeys(string keyDir)
        {
            var files = Directory.EnumerateFiles(keyDir, "*.pfx");

            foreach(var file in files)
            {
                var pwdFile = $"{file}.pwd";
                var pwd = File.ReadAllText(pwdFile).Trim();
                var cert = new X509Certificate2(file, pwd);

                _keys.Add(new X509SecurityKey(cert));
            }
        }
    }
}
