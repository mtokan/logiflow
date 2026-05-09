using LogiFlow.Application.Tracking;

namespace LogiFlow.Application.Abstractions;

public interface ITrackingUpdatePublisher
{
    Task PublishDeliveryPositionUpdatedAsync(
        DeliveryPositionUpdated update,
        CancellationToken cancellationToken = default);
}