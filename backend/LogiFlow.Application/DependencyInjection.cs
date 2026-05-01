using LogiFlow.Application.Abstractions;
using LogiFlow.Application.Eta;
using LogiFlow.Application.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace LogiFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWorkflowEngine, WorkflowEngine>();
        services.AddSingleton<IEtaService, EtaService>();
        return services;
    }
}