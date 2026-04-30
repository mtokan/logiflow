using LogiFlow.Application.Abstractions;
using LogiFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LogiFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDeliveryRepository, InMemoryDeliveryRepository>();
        services.AddSingleton<IDeliveryEventRepository, InMemoryDeliveryEventRepository>();
        return services;
    }
}