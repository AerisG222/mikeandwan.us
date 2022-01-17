using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace MawAuth.Services;

public static class IIdentityServerBuilderExtensions
{
    public static IIdentityServerBuilder AddMawIdentityServerKeyMaterial(this IIdentityServerBuilder builder, string signingCertDir)
    {
#pragma warning disable CA2000
        builder = builder.AddSigningCredential(LoadCertificate(Path.Combine(signingCertDir, "signing.pfx")));
#pragma warning restore CA2000

        return AddValidationCertificates(builder, signingCertDir);
    }

    static IIdentityServerBuilder AddValidationCertificates(IIdentityServerBuilder builder, string signingCertDir)
    {
        var files = Directory.EnumerateFiles(signingCertDir, "*.pfx");

        foreach(var file in files)
        {
#pragma warning disable CA2000
            builder = builder.AddValidationKey(LoadCertificate(file));
#pragma warning restore CA2000
        }

        return builder;
    }

    static X509Certificate2 LoadCertificate(string pfxFile)
    {
        var pwdFile = $"{pfxFile}.pwd";
        var pwd = File.ReadAllText(pwdFile).Trim();

        return new X509Certificate2(pfxFile, pwd);
    }
}
