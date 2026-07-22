using System.Collections.Concurrent;
using System.Text.Json;

namespace NRN.Web.Features.Simulations.Services;

public sealed class JsonSimulationLocalizationService(
    IWebHostEnvironment environment) : ISimulationLocalizationService
{
    private readonly ConcurrentDictionary<
        string,
        IReadOnlyDictionary<string, string>> _cache = new();

    public async Task<string> GetAsync(
        string key,
        string locale,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        locale = NormalizeLocale(locale);

        var translations = await GetTranslationsAsync(
            locale,
            cancellationToken);

        if (translations.TryGetValue(key, out var value))
        {
            return value;
        }

        if (locale != "en")
        {
            var fallback = await GetTranslationsAsync(
                "en",
                cancellationToken);

            if (fallback.TryGetValue(key, out value))
            {
                return value;
            }
        }

        return key;
    }

    private async Task<IReadOnlyDictionary<string, string>>
        GetTranslationsAsync(
            string locale,
            CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(locale, out var cached))
        {
            return cached;
        }

        var path = Path.Combine(
            environment.ContentRootPath,
            "Features",
            "Simulations",
            "Resources",
            "Localization",
            $"{locale}.json");

        if (!File.Exists(path))
        {
            return new Dictionary<string, string>();
        }

        await using var stream = File.OpenRead(path);

        var translations =
            await JsonSerializer.DeserializeAsync<
                Dictionary<string, string>>(
                stream,
                cancellationToken: cancellationToken)
            ?? new Dictionary<string, string>();

        _cache[locale] = translations;

        return translations;
    }

    private static string NormalizeLocale(string locale)
    {
        if (string.IsNullOrWhiteSpace(locale))
        {
            return "en";
        }

        return locale
            .Split('-', StringSplitOptions.RemoveEmptyEntries)[0]
            .ToLowerInvariant();
    }
}
