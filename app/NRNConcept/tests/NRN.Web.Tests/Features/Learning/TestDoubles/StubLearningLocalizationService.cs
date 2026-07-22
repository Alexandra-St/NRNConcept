using NRN.Web.Features.Learning.Services;

namespace NRN.Web.Tests.Features.Learning.TestDoubles;

internal sealed class StubLearningLocalizationService(params string[] missingKeys) : ILearningLocalizationService
{
    private readonly HashSet<string> _missingKeys = missingKeys.ToHashSet(StringComparer.Ordinal);

    public Task<string> GetAsync(string key, string locale, CancellationToken cancellationToken = default) =>
        Task.FromResult(_missingKeys.Contains(key) ? key : $"{locale}:{key}");
}
