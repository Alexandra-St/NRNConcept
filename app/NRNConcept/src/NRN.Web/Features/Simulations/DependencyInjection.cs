using Microsoft.Extensions.DependencyInjection;
using NRN.Web.Features.Simulations.Services;

namespace NRN.Web.Features.Simulations;

public static class DependencyInjection
{
    public static IServiceCollection AddSimulationsFeature(this IServiceCollection services)
    {
        services.AddScoped<SimulationState>();

        services.AddSingleton<ISimulationContentService,
            JsonSimulationContentService>();

        services.AddSingleton<ISimulationLocalizationService,
            JsonSimulationLocalizationService>();

        services.AddSingleton<ISimulationContentValidator,
            SimulationContentValidator>();

        return services;
    }
}
