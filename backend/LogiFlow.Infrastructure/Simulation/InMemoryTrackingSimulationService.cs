using System.Collections.Concurrent;
using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Tracking;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Infrastructure.Simulation;

public class InMemoryTrackingSimulationService(
    IDeliveryRepository deliveryRepository,
    ITrackingUpdatePublisher trackingUpdatePublisher,
    IEtaService etaService,
    IDeliveryRouteRepository routeRepository) : ITrackingSimulationService
{
    private const double DefaultSpeedMetersPerSecond = 12.5;
    private readonly ConcurrentDictionary<Guid, ActiveDeliverySimulation> _activeSimulations = new();

    public IReadOnlyList<ActiveDeliverySimulation> GetActiveSimulations => _activeSimulations.Values.ToList();

    public async Task StartTrackingAsync(Guid deliveryId, CancellationToken cancellationToken = default)
    {
        var delivery = await deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (delivery is null) return;

        var route = await routeRepository.GetByDeliveryIdAsync(deliveryId, cancellationToken);
        if (route is null || route.Points.Count == 0) return;

        var firstPoint = route.Points[0];
        var simulation = new ActiveDeliverySimulation
        {
            DeliveryId = delivery.Id,
            RouteId = route.Id,
            CurrentRoutePointIndex = 0,
            ProgressToNextPoint = 0,
            CurrentPosition = new GeoPoint(firstPoint.Latitude, firstPoint.Longitude),
            CurrentSpeedMetersPerSecond = DefaultSpeedMetersPerSecond
        };
        _activeSimulations[delivery.Id] = simulation;

        delivery.UpdatePosition(simulation.CurrentPosition);
        await deliveryRepository.UpdateAsync(delivery, cancellationToken);
    }

    public Task StopTrackingAsync(Guid deliveryId, CancellationToken cancellationToken = default)
    {
        _activeSimulations.TryRemove(deliveryId, out _);
        return Task.CompletedTask;
    }

    public async Task TickAsync(CancellationToken cancellationToken = default)
    {
        foreach (var simulation in _activeSimulations.Values) await MoveSimulationAsync(simulation, cancellationToken);
    }

    private async Task MoveSimulationAsync(ActiveDeliverySimulation simulation, CancellationToken cancellationToken)
    {
        var route = await routeRepository.GetByDeliveryIdAsync(simulation.DeliveryId, cancellationToken);
        if (route is null || route.Points.Count < 2)
        {
            await StopTrackingAsync(simulation.DeliveryId, cancellationToken);
            return;
        }

        var currentIndex = simulation.CurrentRoutePointIndex;
        var nextIndex = currentIndex + 1;

        if (nextIndex >= route.Points.Count)
        {
            await StopTrackingAsync(simulation.DeliveryId, cancellationToken);
            return;
        }

        var currentPoint = route.Points[currentIndex];
        var nextPoint = route.Points[nextIndex];

        var segmentDistance = nextPoint.DistanceFromPreviousMeters;
        if (segmentDistance <= 0)
        {
            simulation.CurrentRoutePointIndex++;
            simulation.ProgressToNextPoint = 0;
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var elapsedSeconds = Math.Max(0, (now - simulation.LastUpdateAt).TotalSeconds);
        simulation.LastUpdateAt = now;

        var distanceTravelled = simulation.CurrentSpeedMetersPerSecond * elapsedSeconds;
        var progressDelta = distanceTravelled / segmentDistance;

        simulation.ProgressToNextPoint += progressDelta;

        while (simulation.ProgressToNextPoint >= 1)
        {
            simulation.CurrentRoutePointIndex++;
            simulation.ProgressToNextPoint -= 1;
            currentIndex = simulation.CurrentRoutePointIndex;
            nextIndex = currentIndex + 1;
            if (nextIndex >= route.Points.Count)
            {
                var finalPoint = route.Points[^1];
                simulation.CurrentPosition = new GeoPoint(finalPoint.Latitude, finalPoint.Longitude);
                await UpdateDeliveryPositionAsync(simulation.DeliveryId, simulation.CurrentPosition, route, simulation,
                    cancellationToken);
                await StopTrackingAsync(simulation.DeliveryId, cancellationToken);
                return;
            }

            currentPoint = route.Points[currentIndex];
            nextPoint = route.Points[nextIndex];
        }

        var interpolatedPosition = Interpolate(currentPoint.Latitude, currentPoint.Longitude,
            nextPoint.Latitude, nextPoint.Longitude, simulation.ProgressToNextPoint);

        simulation.CurrentPosition = interpolatedPosition;

        await UpdateDeliveryPositionAsync(simulation.DeliveryId, simulation.CurrentPosition, route, simulation,
            cancellationToken);
    }

    private async Task UpdateDeliveryPositionAsync(Guid deliveryId, GeoPoint position, DeliveryRoute route,
        ActiveDeliverySimulation simulation, CancellationToken cancellationToken)
    {
        var delivery = await deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (delivery is null) return;

        var eta = etaService.CalculateEta(route, simulation.CurrentRoutePointIndex,
            simulation.ProgressToNextPoint, simulation.CurrentSpeedMetersPerSecond);

        var progressPercent = etaService.CalculateProgressPercent(route, simulation.CurrentRoutePointIndex,
            simulation.ProgressToNextPoint);

        delivery.UpdatePosition(position);
        delivery.UpdateEta(eta);

        await deliveryRepository.UpdateAsync(delivery, cancellationToken);

        await trackingUpdatePublisher.PublishPositionUpdatedAsync(
            new VehiclePositionUpdated(delivery.Id, position, simulation.CurrentSpeedMetersPerSecond, eta.TotalSeconds,
                progressPercent, delivery.State, DateTimeOffset.UtcNow), cancellationToken);
    }

    private static GeoPoint Interpolate(double fromLatitude, double fromLongitude, double toLatitude,
        double toLongitude, double progress)
    {
        var latitude = fromLatitude + (toLatitude - fromLatitude) * progress;
        var longitude = fromLongitude + (toLongitude - fromLongitude) * progress;
        return new GeoPoint(latitude, longitude);
    }
}