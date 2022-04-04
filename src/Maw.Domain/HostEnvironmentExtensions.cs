namespace Microsoft.Extensions.Hosting;

public static class HostEnvironmentExtensions
{
    public static bool IsStage(this IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment == null)
        {
            throw new ArgumentNullException(nameof(hostEnvironment));
        }

        return hostEnvironment.IsEnvironment("stage");
    }

    public static bool IsTest(this IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment == null)
        {
            throw new ArgumentNullException(nameof(hostEnvironment));
        }

        return hostEnvironment.IsEnvironment("test");
    }
}
