using LogiFlow.Domain.Entities;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Api.Contracts;

public static class ApiContractMapper
{
    public static DeliveryResponse ToResponse(this Delivery delivery)
    {
        return new DeliveryResponse(
            delivery.Id,
            delivery.Code,
            delivery.Origin.ToResponse(),
            delivery.Destination.ToResponse(),
            delivery.CurrentPosition?.ToResponse(),
            delivery.State,
            delivery.AssignedVehicleId,
            delivery.RouteId,
            delivery.Eta?.TotalSeconds,
            delivery.CreatedAt,
            delivery.UpdatedAt);
    }

    public static VehicleResponse ToResponse(this Vehicle vehicle)
    {
        return new VehicleResponse(
            vehicle.Id,
            vehicle.Name,
            vehicle.Status,
            vehicle.AssignedDeliveryId,
            vehicle.IsActive);
    }

    public static DeliveryRouteResponse ToResponse(this DeliveryRoute route)
    {
        return new DeliveryRouteResponse(
            route.Id,
            route.DeliveryId,
            route.Points
                .OrderBy(point => point.Sequence)
                .Select(point => new RoutePointResponse(
                    point.Latitude,
                    point.Longitude,
                    point.Sequence,
                    point.DistanceFromPreviousMeters))
                .ToList(),
            route.TrafficSegments
                .Select(segment => new TrafficSegmentResponse(
                    segment.FromRoutePointIndex,
                    segment.ToRoutePointIndex,
                    segment.Level,
                    segment.SpeedMultiplier))
                .ToList(),
            route.TotalDistanceMeters,
            route.EstimatedDuration.TotalSeconds);
    }

    public static DeliveryEventResponse ToResponse(this DeliveryEvent deliveryEvent)
    {
        return new DeliveryEventResponse(
            deliveryEvent.Id,
            deliveryEvent.DeliveryId,
            deliveryEvent.Type,
            deliveryEvent.FromState,
            deliveryEvent.ToState,
            deliveryEvent.Message,
            deliveryEvent.CreatedAt);
    }

    private static GeoPointResponse ToResponse(this GeoPoint point)
    {
        return new GeoPointResponse(
            point.Latitude,
            point.Longitude);
    }
}