using System.Collections.Concurrent;
using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Tracking;
using LogiFlow.Application.Workflow;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Infrastructure.Simulation;

public sealed class InMemoryTrackingSimulationService(
    IDeliveryRepository deliveryRepository,
    IVehicleRepository vehicleRepository,
    IDeliveryTrackingPolicy deliveryTrackingPolicy,
    ITrackingUpdatePublisher trackingUpdatePublisher,
    IEtaService etaService,
    ITrafficSimulationService trafficSimulationService,
    IWorkflowEngine workflowEngine,
    IDeliveryRouteRepository routeRepository) : ITrackingSimulationService
{
    private const double DefaultSpeedMetersPerSecond = 12.5;

    private readonly ConcurrentDictionary<Guid, ActiveDeliverySimulation> _activeSimulations = new();

    public async Task StartTrackingAsync(Guid deliveryId, CancellationToken cancellationToken = default)
    {
        if (_activeSimulations.ContainsKey(deliveryId)) return;

        var delivery = await deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (delivery?.AssignedVehicleId is null) return;

        var route = await routeRepository.GetByDeliveryIdAsync(deliveryId, cancellationToken);
        if (route is null || route.Points.Count < 2) return;

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

        var delivery = await deliveryRepository.GetByIdAsync(simulation.DeliveryId, cancellationToken);
        if (delivery is null || delivery.State is DeliveryState.Delivered or DeliveryState.Closed ||
            delivery.AssignedVehicleId is not { } vehicleId)
        {
            await StopTrackingAsync(simulation.DeliveryId, cancellationToken);
            return;
        }

        var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null)
        {
            await StopTrackingAsync(delivery.Id, cancellationToken);
            return;
        }

        MoveAlongRoute(simulation, route);

        simulation.CurrentPosition = CalculateCurrentPosition(route, simulation);

        var adjustedSpeedMetersPerSecond = CalculateAdjustedSpeedMetersPerSecond(simulation, route);

        var eta = etaService.CalculateEta(route, simulation.CurrentRoutePointIndex,
            simulation.ProgressToNextPoint, simulation.CurrentSpeedMetersPerSecond);

        var progressPercent = etaService.CalculateProgressPercent(route, simulation.CurrentRoutePointIndex,
            simulation.ProgressToNextPoint);

        var isAtFinalPoint = IsAtFinalPoint(simulation, route);

        var nextState = deliveryTrackingPolicy.DetermineNextState(delivery.State, progressPercent, eta,
            adjustedSpeedMetersPerSecond, isAtFinalPoint);

        if (nextState is not null)
        {
            var transitionResult = await workflowEngine.TransitionAsync(delivery.Id, nextState.Value,
                cancellationToken);

            delivery = transitionResult.Delivery;
        }

        delivery.UpdatePosition(simulation.CurrentPosition);
        delivery.UpdateEta(eta);

        await deliveryRepository.UpdateAsync(delivery, cancellationToken);

        vehicle.UpdatePosition(simulation.CurrentPosition, adjustedSpeedMetersPerSecond * 3.6);
        await vehicleRepository.UpdateAsync(vehicle, cancellationToken);

        await trackingUpdatePublisher.PublishPositionUpdatedAsync(new VehiclePositionUpdated(
            delivery.Id,
            vehicleId,
            simulation.CurrentPosition,
            adjustedSpeedMetersPerSecond,
            eta.TotalSeconds,
            progressPercent,
            delivery.State,
            DateTimeOffset.UtcNow
        ), cancellationToken);

        if (delivery.State is DeliveryState.Delivered or DeliveryState.Closed)
        {
            vehicle.Release();
            
            await vehicleRepository.UpdateAsync(vehicle, cancellationToken);
            await StopTrackingAsync(delivery.Id, cancellationToken);
        }
    }

    private static double CalculateElapsedSeconds(ActiveDeliverySimulation simulation)
    {
        var now = DateTimeOffset.UtcNow;

        var elapsedSeconds = Math.Max(0, (now - simulation.LastUpdateAt).TotalSeconds);
        simulation.LastUpdateAt = now;

        return elapsedSeconds;
    }

    private void MoveAlongRoute(ActiveDeliverySimulation simulation, DeliveryRoute route)
    {
        var remainingTimeSeconds = CalculateElapsedSeconds(simulation);
        while (remainingTimeSeconds > 0)
        {
            if (IsAtFinalPoint(simulation, route))
            {
                simulation.ProgressToNextPoint = 0;
                return;
            }

            var currentIndex = simulation.CurrentRoutePointIndex;
            var nextIndex = currentIndex + 1;

            var nextPoint = route.Points[nextIndex];
            var segmentDistanceMeters = nextPoint.DistanceFromPreviousMeters;

            if (segmentDistanceMeters <= 0)
            {
                simulation.CurrentRoutePointIndex++;
                simulation.ProgressToNextPoint = 0;
                continue;
            }

            var adjustedSpeedMetersPerSecond = CalculateAdjustedSpeedMetersPerSecond(simulation, route);
            if (adjustedSpeedMetersPerSecond <= 0) return;

            var remainingSegmentDistanceMeters = segmentDistanceMeters * (1 - simulation.ProgressToNextPoint);

            var timeToFinishSegmentSeconds = remainingSegmentDistanceMeters / adjustedSpeedMetersPerSecond;

            if (remainingTimeSeconds >= timeToFinishSegmentSeconds)
            {
                remainingTimeSeconds -= timeToFinishSegmentSeconds;
                simulation.CurrentRoutePointIndex++;
                simulation.ProgressToNextPoint = 0;
            }
            else
            {
                var distanceTravelledMeters = remainingTimeSeconds * adjustedSpeedMetersPerSecond;
                simulation.ProgressToNextPoint += distanceTravelledMeters / segmentDistanceMeters;
                remainingTimeSeconds = 0;
            }
        }
    }

    private static bool IsAtFinalPoint(ActiveDeliverySimulation simulation, DeliveryRoute route)
    {
        return simulation.CurrentRoutePointIndex >= route.Points.Count - 1;
    }

    private static GeoPoint CalculateCurrentPosition(DeliveryRoute route, ActiveDeliverySimulation simulation)
    {
        if (IsAtFinalPoint(simulation, route))
        {
            var finalPoint = route.Points[^1];
            return new GeoPoint(finalPoint.Latitude, finalPoint.Longitude);
        }

        var currentPoint = route.Points[simulation.CurrentRoutePointIndex];
        var nextPoint = route.Points[simulation.CurrentRoutePointIndex + 1];

        return Interpolate(currentPoint.Latitude, currentPoint.Longitude, nextPoint.Latitude, nextPoint.Longitude,
            simulation.ProgressToNextPoint);
    }

    private double CalculateAdjustedSpeedMetersPerSecond(ActiveDeliverySimulation simulation, DeliveryRoute route)
    {
        if (IsAtFinalPoint(simulation, route)) return 0;
        var trafficMultiplier = trafficSimulationService.GetSpeedMultiplier(route.TrafficSegments,
            simulation.CurrentRoutePointIndex);
        return simulation.CurrentSpeedMetersPerSecond * trafficMultiplier;
    }

    private static GeoPoint Interpolate(double fromLatitude, double fromLongitude, double toLatitude,
        double toLongitude, double progress)
    {
        var clampedProgress = Math.Clamp(progress, 0, 1);

        var latitude = fromLatitude + (toLatitude - fromLatitude) * clampedProgress;
        var longitude = fromLongitude + (toLongitude - fromLongitude) * clampedProgress;
        return new GeoPoint(latitude, longitude);
    }
}