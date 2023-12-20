namespace Microsoft.Extensions.Hosting;

public static class HostEnvironmentExtensions
{
    public static bool IsStage(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment("stage");
    }

    public static bool IsTest(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment("test");
    }
}
