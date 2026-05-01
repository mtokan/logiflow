using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Eta;

public sealed class EtaService : IEtaService
{
    public TimeSpan CalculateEta(DeliveryRoute route, int currentRoutePointIndex, double progressToNextPoint,
        double speedMetersPerSecond)
    {
        if (route.Points.Count < 2 || speedMetersPerSecond <= 0) return TimeSpan.Zero;

        var remainingDistance = CalculateRemainingDistance(route, currentRoutePointIndex, progressToNextPoint);
        var etaSeconds = remainingDistance / speedMetersPerSecond;

        return TimeSpan.FromSeconds(Math.Max(0, etaSeconds));
    }

    public double CalculateProgressPercent(DeliveryRoute route, int currentRoutePointIndex, double progressToNextPoint)
    {
        if (route.Points.Count < 2 || route.TotalDistanceMeters <= 0) return 100;

        var remainingDistance = CalculateRemainingDistance(route, currentRoutePointIndex, progressToNextPoint);
        var completedDistance = route.TotalDistanceMeters - remainingDistance;
        var progressPercent = completedDistance / route.TotalDistanceMeters * 100;

        return Math.Clamp(progressPercent, 0, 100);
    }

    private static double CalculateRemainingDistance(DeliveryRoute route, int currentRoutePointIndex,
        double progressToNextPoint)
    {
        var nextIndex = currentRoutePointIndex + 1;
        if (nextIndex >= route.Points.Count) return 0;

        var currentSegmentDistance = route.Points[nextIndex].DistanceFromPreviousMeters;
        var remainingCurrentSegmentDistance = currentSegmentDistance * (1 - progressToNextPoint);
        var remainingFutureSegmentsDistance = route.Points.Skip(nextIndex + 1)
            .Sum(point => point.DistanceFromPreviousMeters);

        return remainingCurrentSegmentDistance + remainingFutureSegmentsDistance;
    }
}