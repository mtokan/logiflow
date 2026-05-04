using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Infrastructure.Routing;

public class MockRoutingService(ITrafficSimulationService trafficSimulationService) : IRoutingService
{
    private const int NumberOfPoints = 25;
    private const double AverageSpeedMetersPerSecond = 12.5;

    public Task<DeliveryRoute> CalculateRouteAsync(Guid deliveryId, GeoPoint origin, GeoPoint destination,
        CancellationToken cancellationToken = default)
    {
        var points = new List<RoutePoint>();
        double totalDistance = 0;
        GeoPoint? previousPoint = null;

        for (var i = 0; i < NumberOfPoints; i++)
        {
            var ratio = (double)i / (NumberOfPoints - 1);

            var latitude = origin.Latitude + (destination.Latitude - origin.Latitude) * ratio;
            var longitude = origin.Longitude + (destination.Longitude - origin.Longitude) * ratio;

            var currentPoint = new GeoPoint(latitude, longitude);

            var distanceFromPrevious = previousPoint is null
                ? 0
                : CalculateDistanceMeters(previousPoint, currentPoint);

            totalDistance += distanceFromPrevious;

            points.Add(new RoutePoint(latitude, longitude, i, distanceFromPrevious));
            previousPoint = currentPoint;
        }

        var estimatedDuration = TimeSpan.FromSeconds(totalDistance / AverageSpeedMetersPerSecond);

        var trafficSegments = trafficSimulationService.GenerateTrafficSegments(points.Count);

        var route = new DeliveryRoute
        {
            DeliveryId = deliveryId,
            Points = points,
            TotalDistanceMeters = totalDistance,
            TrafficSegments = trafficSegments,
            EstimatedDuration = estimatedDuration
        };

        return Task.FromResult(route);
    }

    private static double CalculateDistanceMeters(GeoPoint from, GeoPoint to)
    {
        const double earthRadiusMeters = 6_371_000;
        var lat1 = DegreesToRadians(from.Latitude);
        var lat2 = DegreesToRadians(to.Latitude);
        var deltaLat = DegreesToRadians(to.Latitude - from.Latitude);
        var deltaLon = DegreesToRadians(to.Longitude - from.Longitude);
        var a =
            Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
            Math.Cos(lat1) * Math.Cos(lat2) *
            Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusMeters * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}