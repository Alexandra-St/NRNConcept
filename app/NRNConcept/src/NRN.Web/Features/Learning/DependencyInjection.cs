using NRN.Web.Features.Learning.Services;

namespace NRN.Web.Features.Learning;

public static class DependencyInjection
{
    public static IServiceCollection AddLearningFeature(
        this IServiceCollection services)
    {
        services.AddScoped<LearningState>();

        services.AddSingleton<ILearningContentService,
            JsonLearningContentService>();

        services.AddSingleton<ILearningLocalizationService,
            JsonLearningLocalizationService>();

        services.AddSingleton<ILearningContentValidator,
            LearningContentValidator>();

        return services;
    }
}
