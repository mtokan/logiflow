using LogiFlow.Domain.Entities;

namespace LogiFlow.Application.Abstractions;

public interface IEtaService
{
    TimeSpan CalculateEta(
        DeliveryRoute route,
        int currentRoutePointIndex,
        double progressToNextPoint,
        double speedMetersPerSecond);

    double CalculateProgressPercent(
        DeliveryRoute route,
        int currentRoutePointIndex,
        double progressToNextPoint);
}