using LogiFlow.Api.Contracts;
using LogiFlow.Domain.Entities;

namespace LogiFlow.Api.Realtime.Contracts;

public static class RealtimeContractMapper
{
    public static DeliverySnapshotUpdated ToDeliverySnapshotUpdated(this Delivery delivery, string reason)
    {
        return new DeliverySnapshotUpdated(delivery.ToResponse(), reason, DateTimeOffset.UtcNow);
    }

    public static VehicleSnapshotUpdated ToVehicleSnapshotUpdated(this Vehicle vehicle, string reason)
    {
        return new VehicleSnapshotUpdated(vehicle.ToResponse(), reason, DateTimeOffset.UtcNow);
    }

    public static DeliveryLogEventCreated ToDeliveryLogEventCreated(this DeliveryEvent deliveryEvent)
    {
        return new DeliveryLogEventCreated(
            deliveryEvent.Id,
            deliveryEvent.DeliveryId,
            deliveryEvent.Type,
            deliveryEvent.FromState,
            deliveryEvent.ToState,
            deliveryEvent.Message,
            deliveryEvent.CreatedAt);
    }

    public static WarehouseSnapshotUpdated ToWarehouseSnapshotUpdated(this Warehouse warehouse, string reason)
    {
        return new WarehouseSnapshotUpdated(warehouse.ToResponse(), reason, DateTimeOffset.UtcNow);
    }
}