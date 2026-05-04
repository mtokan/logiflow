using LogiFlow.Domain.ValueObjects;

namespace LogiFlow.Application.Abstractions;

public interface ITrafficSimulationService
{
    IReadOnlyList<TrafficSegment> GenerateTrafficSegments(int routePointCount);
    double GetSpeedMultiplier(IReadOnlyList<TrafficSegment> trafficSegments, int currentRoutePointIndex);
}