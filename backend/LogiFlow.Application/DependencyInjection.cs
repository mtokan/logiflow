using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Eta;
using LogiFlow.Application.Events;
using LogiFlow.Application.Tracking;
using LogiFlow.Application.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace LogiFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IWorkflowEngine, WorkflowEngine>();
        services.AddSingleton<IEtaService, EtaService>();
        services.AddSingleton<IDeliveryEventService, DeliveryEventService>();
        services.AddSingleton<IDeliveryTrackingPolicy, DeliveryTrackingPolicy>();
        return services;
    }
}