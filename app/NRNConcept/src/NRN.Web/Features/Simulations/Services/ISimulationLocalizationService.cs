namespace NRN.Web.Features.Simulations.Services;

public interface ISimulationLocalizationService
{
    Task<string> GetAsync(
        string key,
        string locale,
        CancellationToken cancellationToken = default);
}
