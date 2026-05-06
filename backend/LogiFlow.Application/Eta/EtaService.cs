using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Eta;

public sealed class EtaService : IEtaService
{
    public TimeSpan CalculateEta(DeliveryRoute route, int currentRoutePointIndex, double progressToNextPoint,
        double baseSpeedMetersPerSecond)
    {
        if (route.Points.Count < 2 || baseSpeedMetersPerSecond <= 0) return TimeSpan.MaxValue;
        if (currentRoutePointIndex >= route.Points.Count - 1) return TimeSpan.Zero;

        var totalSeconds = 0.0;

        for (var pointIndex = currentRoutePointIndex + 1; pointIndex < route.Points.Count; pointIndex++)
        {
            var segmentDistanceMeters = route.Points[pointIndex].DistanceFromPreviousMeters;
            if (segmentDistanceMeters <= 0) continue;

            var segmentStartIndex = pointIndex - 1;
            var trafficMultiplier = GetTrafficMultiplier(route, segmentStartIndex);
            var segmentSpeedMetersPerSecond = baseSpeedMetersPerSecond * trafficMultiplier;
            if (segmentSpeedMetersPerSecond <= 0) return TimeSpan.MaxValue;

            var remainingSegmentDistanceMeters = segmentDistanceMeters;
            if (segmentStartIndex == currentRoutePointIndex)
                remainingSegmentDistanceMeters = segmentDistanceMeters * (1 - Math.Clamp(progressToNextPoint, 0, 1));

            totalSeconds += remainingSegmentDistanceMeters / segmentSpeedMetersPerSecond;
        }

        return TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
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

        var progress = Math.Clamp(progressToNextPoint, 0, 1);

        var currentSegmentDistance = route.Points[nextIndex].DistanceFromPreviousMeters;
        var remainingCurrentSegmentDistance = currentSegmentDistance * (1 - progress);
        var remainingFutureSegmentsDistance = route.Points.Skip(nextIndex + 1)
            .Sum(point => point.DistanceFromPreviousMeters);

        return remainingCurrentSegmentDistance + remainingFutureSegmentsDistance;
    }

    private static double GetTrafficMultiplier(DeliveryRoute route, int segmentStartIndex)
    {
        var segment = route.TrafficSegments.FirstOrDefault(segment => segment.FromRoutePointIndex == segmentStartIndex);
        return segment?.SpeedMultiplier ?? 1.0;
    }
}