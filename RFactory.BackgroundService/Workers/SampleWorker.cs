using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RFactory.BackgroundService.Workers;

/// <summary>
/// Sample long-running background worker demonstrating the hosted service pattern.
/// Replace or extend for real OEE workers, alert evaluators, etc.
/// </summary>
public class SampleWorker : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly ILogger<SampleWorker> _logger;

    public SampleWorker(ILogger<SampleWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SampleWorker started at {time}", DateTimeOffset.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug("SampleWorker heartbeat at {time}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("SampleWorker stopped at {time}", DateTimeOffset.UtcNow);
    }
}
