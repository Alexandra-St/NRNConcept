namespace NRN.Web.Features.Learning.Services;

public sealed class LearningContentValidator(
    ILearningContentService contentService,
    ILearningLocalizationService localizationService) : ILearningContentValidator
{
    private static readonly string[] Locales = ["en", "ru"];

    public async Task ValidateAsync(CancellationToken cancellationToken = default)
    {
        var categories = await contentService.GetCategoriesAsync(cancellationToken);
        var topics = await contentService.GetTopicsAsync(cancellationToken);
        var products = await contentService.GetProductsAsync(cancellationToken);
        var sources = await contentService.GetSourcesAsync(cancellationToken);
        var errors = new List<string>();

        ValidateUnique(categories.Select(item => item.Id), "category id", errors);
        ValidateUnique(topics.Select(item => item.Slug), "topic slug", errors);
        ValidateUnique(products.Select(item => item.Id), "product id", errors);
        ValidateUnique(sources.Select(item => item.Id), "source id", errors);

        var categoryIds = categories.Select(item => item.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var productIds = products.Select(item => item.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var sourceIds = sources.Select(item => item.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var topic in topics)
        {
            if (!categoryIds.Contains(topic.CategoryId))
                errors.Add($"Topic '{topic.Slug}' references missing category '{topic.CategoryId}'.");

            foreach (var productId in topic.RelatedProductIds ?? [])
                if (!productIds.Contains(productId)) errors.Add($"Topic '{topic.Slug}' references missing product '{productId}'.");

            foreach (var sourceId in topic.RelatedSourceIds ?? [])
                if (!sourceIds.Contains(sourceId)) errors.Add($"Topic '{topic.Slug}' references missing source '{sourceId}'.");
        }

        foreach (var group in topics.GroupBy(item => item.CategoryId, StringComparer.OrdinalIgnoreCase))
            ValidateUnique(group.Select(item => item.Order.ToString()), $"topic order in category '{group.Key}'", errors);

        foreach (var source in sources)
            if (!Uri.TryCreate(source.Url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                errors.Add($"Source '{source.Id}' must use a valid HTTPS URL.");

        var localizationKeys = categories.SelectMany(item => new[] { item.NameKey, item.DescriptionKey })
            .Concat(topics.SelectMany(item => new[] { item.TitleKey, item.TaglineKey, item.SummaryKey, item.SituationKey, item.ProblemKey, item.RealityKey, item.SolutionKey }))
            .Concat(products.SelectMany(item => new[] { item.NameKey, item.BenefitKey, item.LimitationKey }))
            .Distinct(StringComparer.Ordinal);

        foreach (var locale in Locales)
        foreach (var key in localizationKeys)
            if (string.Equals(await localizationService.GetAsync(key, locale, cancellationToken), key, StringComparison.Ordinal))
                errors.Add($"Missing localization key '{key}' for locale '{locale}'.");

        if (errors.Count > 0)
            throw new InvalidOperationException("Learning content validation failed:\n- " + string.Join("\n- ", errors));
    }

    private static void ValidateUnique(IEnumerable<string> values, string label, ICollection<string> errors)
    {
        foreach (var duplicate in values.GroupBy(value => value, StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1))
            errors.Add($"Duplicate {label}: '{duplicate.Key}'.");
    }
}
