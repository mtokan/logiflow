using LogiFlow.Application.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogiFlow.Infrastructure.Simulation;

public sealed class DeliverySimulationBackgroundService(
    ITrackingSimulationService trackingSimulationService,
    ILogger<DeliverySimulationBackgroundService> logger)
    : BackgroundService
{
    private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Delivery simulation background service started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await trackingSimulationService.TickAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Error occurred during delivery simulation.");
                }

                await Task.Delay(TickInterval, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }

        logger.LogInformation("Delivery simulation background service stopped.");
    }
}