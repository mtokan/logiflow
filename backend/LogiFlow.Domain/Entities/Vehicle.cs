using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Domain.Entities;

public sealed class Vehicle
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public GeoPoint? CurrentPosition { get; private set; }
    public double CurrentSpeedKmh { get; private set; }
    public bool IsActive { get; private set; }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        CurrentSpeedKmh = 0;
    }

    public void UpdatePosition(GeoPoint position, double speedKmh)
    {
        CurrentPosition = position;
        CurrentSpeedKmh = speedKmh;
    }
}