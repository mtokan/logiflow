using LogiFlow.Application.Abstractions;
using LogiFlow.Infrastructure.Persistence;
using LogiFlow.Infrastructure.Routing;
using LogiFlow.Infrastructure.Simulation;
using LogiFlow.Infrastructure.Traffic;
using Microsoft.Extensions.DependencyInjection;

namespace LogiFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDeliveryRepository, InMemoryDeliveryRepository>();
        services.AddSingleton<IDeliveryEventRepository, InMemoryDeliveryEventRepository>();
        services.AddSingleton<IDeliveryRouteRepository, InMemoryDeliveryRouteRepository>();
        services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();

        services.AddSingleton<ITrafficSimulationService, TrafficSimulationService>();
        services.AddSingleton<IRoutingService, MockRoutingService>();
        services.AddSingleton<ITrackingSimulationService, InMemoryTrackingSimulationService>();

        services.AddHostedService<DeliverySimulationBackgroundService>();
        return services;
    }
}