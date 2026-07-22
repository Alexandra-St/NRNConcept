using NRN.Web.Features.Simulations.Services;

namespace NRN.Web.Tests.Features.Simulations.TestDoubles;

internal sealed class StubSimulationLocalizationService(
    params string[] missingKeys) : ISimulationLocalizationService
{
    private readonly HashSet<string> _missingKeys =
        missingKeys.ToHashSet(StringComparer.Ordinal);

    public Task<string> GetAsync(
        string key,
        string locale,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(
            _missingKeys.Contains(key) ? key : $"{locale}:{key}");
}
