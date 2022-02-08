namespace Maw.Cache.Initializer;

public class Worker
    : BackgroundService
{
    readonly IServiceProvider _services;
    readonly ILogger _logger;

    public Worker(
        IServiceProvider services,
        ILogger<Worker> logger)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker starting at: {time}", DateTimeOffset.Now);

            await StartServicesAsync(stoppingToken);
        }
    }

    async Task StartServicesAsync(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();

        try
        {
            var processors = scope.ServiceProvider.GetServices<IScopedProcessingService>();
            var tasks = new List<Task>();

            foreach(var processor in processors)
            {
                tasks.Add(processor.DoWorkAsync(stoppingToken));
            }

            await Task.WhenAll(tasks.ToArray());
        }
        catch(Exception ex)
        {
            _logger.LogWarning(ex, "Error starting service");
        }
    }
}
