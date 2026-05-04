using LogiFlow.Application.Abstractions;
using LogiFlow.Domain.Enums;
using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Infrastructure.Traffic;

public sealed class TrafficSimulationService : ITrafficSimulationService
{
    private static readonly Random Random = new();

    public IReadOnlyList<TrafficSegment> GenerateTrafficSegments(int routePointCount)
    {
        if (routePointCount < 2) return [];

        var segments = new List<TrafficSegment>();

        for (var i = 0; i < routePointCount - 1; i++)
        {
            var level = GetRandomTrafficLevel();
            var multiplier = GetMultiplier(level);

            segments.Add(new TrafficSegment(
                i,
                i + 1,
                level,
                multiplier));
        }

        return segments;
    }

    public double GetSpeedMultiplier(IReadOnlyList<TrafficSegment> trafficSegments, int currentRoutePointIndex)
    {
        var segment = trafficSegments.FirstOrDefault(segment => segment.FromRoutePointIndex == currentRoutePointIndex);
        return segment?.SpeedMultiplier ?? 1.0;
    }

    private static TrafficLevel GetRandomTrafficLevel()
    {
        var value = Random.Next(1, 101);

        return value switch
        {
            <= 60 => TrafficLevel.Low,
            <= 85 => TrafficLevel.Medium,
            _ => TrafficLevel.High
        };
    }

    private static double GetMultiplier(TrafficLevel level)
    {
        return level switch
        {
            TrafficLevel.Low => 1.0,
            TrafficLevel.Medium => 0.65,
            TrafficLevel.High => 0.35,
            _ => 1.0
        };
    }
}