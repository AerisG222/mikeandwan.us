namespace Maw.Cache.Sync;

// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=netcore-cli#consuming-a-scoped-service-in-a-background-task
internal interface IScopedProcessingService
{
    Task DoWorkAsync(CancellationToken stoppingToken);
}
