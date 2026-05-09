using LogiFlow.Domain.Enums;

namespace LogiFlow.Domain.Entities;

public sealed class Vehicle
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public VehicleStatus Status { get; private set; } = VehicleStatus.Available;
    public Guid? AssignedDeliveryId { get; private set; }
    public bool IsActive => Status == VehicleStatus.InTransit;

    public void AssignToDelivery(Guid deliveryId)
    {
        if (Status != VehicleStatus.Available)
            throw new InvalidOperationException("Vehicle is not available.");

        AssignedDeliveryId = deliveryId;
        Status = VehicleStatus.Assigned;
    }

    public void MarkInTransit()
    {
        if (Status != VehicleStatus.Assigned)
            throw new InvalidOperationException("Vehicle must be assigned before it can start transit.");

        Status = VehicleStatus.InTransit;
    }

    public void Release()
    {
        AssignedDeliveryId = null;
        Status = VehicleStatus.Available;
    }

    public void MarkUnavailable()
    {
        Status = VehicleStatus.Unavailable;
    }
}