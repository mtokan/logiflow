using LogiFlow.Domain.Entities;

namespace LogiFlow.Api.Realtime;

public interface IRealtimeUpdatePublisher
{
    Task PublishDeliveryLogEventAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken = default);

    Task PublishDeliverySnapshotAsync(Delivery delivery, string reason, CancellationToken cancellationToken = default);

    Task PublishVehicleSnapshotAsync(Vehicle vehicle, string reason, CancellationToken cancellationToken = default);
}