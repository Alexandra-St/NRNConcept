namespace NRN.Web.Features.Learning.Services;

public interface ILearningLocalizationService
{
    Task<string> GetAsync(string key,
        string locale,
        CancellationToken cancellationToken = default);
}
