using LogiFlow.Application.Tracking;

namespace LogiFlow.Application.Abstractions;

public interface ITrackingUpdatePublisher
{
    Task PublishPositionUpdatedAsync(VehiclePositionUpdated update, CancellationToken cancellationToken = default);
    Task PublishDeliveryStateChangedAsync(DeliveryStateChanged update, CancellationToken cancellationToken = default);
    Task PublishDeliveryEventCreatedAsync(DeliveryEventCreated update, CancellationToken cancellationToken = default);
}