using LogiFlow.Api.Hubs;
using LogiFlow.Api.Realtime.Contracts;
using LogiFlow.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace LogiFlow.Api.Realtime;

public sealed class SignalRRealtimeUpdatePublisher(
    IHubContext<TrackingHub> hubContext)
    : IRealtimeUpdatePublisher
{
    public Task PublishDeliveryLogEventAsync(DeliveryEvent deliveryEvent, CancellationToken cancellationToken = default)
    {
        return hubContext.Clients.All.SendAsync(
            "DeliveryLogEventCreated",
            deliveryEvent.ToDeliveryLogEventCreated(),
            cancellationToken);
    }

    public Task PublishDeliverySnapshotAsync(Delivery delivery, string reason,
        CancellationToken cancellationToken = default)
    {
        return hubContext.Clients.All.SendAsync(
            "DeliverySnapshotUpdated",
            delivery.ToDeliverySnapshotUpdated(reason),
            cancellationToken);
    }

    public Task PublishVehicleSnapshotAsync(Vehicle vehicle, string reason,
        CancellationToken cancellationToken = default)
    {
        return hubContext.Clients.All.SendAsync(
            "VehicleSnapshotUpdated",
            vehicle.ToVehicleSnapshotUpdated(reason),
            cancellationToken);
    }
}