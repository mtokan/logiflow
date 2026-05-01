using LogiFlow.Application.Tracking;

namespace LogiFlow.Application.Abstractions;

public interface ITrackingUpdatePublisher
{
    Task PublishPositionUpdatedAsync(VehiclePositionUpdated update, CancellationToken cancellationToken = default);
}