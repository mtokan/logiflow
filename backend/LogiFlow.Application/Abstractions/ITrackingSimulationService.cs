namespace LogiFlow.Application.Abstractions;

public interface ITrackingSimulationService
{
    Task StartTrackingAsync(Guid deliveryId, CancellationToken cancellationToken = default);
    Task StopTrackingAsync(Guid deliveryId, CancellationToken cancellationToken = default);
    Task TickAsync(CancellationToken cancellationToken = default);
}