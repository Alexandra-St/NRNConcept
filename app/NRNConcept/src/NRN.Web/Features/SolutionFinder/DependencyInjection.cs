using NRN.Web.Features.SolutionFinder.Services;

namespace NRN.Web.Features.SolutionFinder;

public static class DependencyInjection
{
    public static IServiceCollection AddSolutionFinderFeature(this IServiceCollection services)
    {
        services.AddScoped<SolutionFinderState>();
        services.AddSingleton<ISolutionFinderService, SolutionFinderService>();

        return services;
    }
}
