using System.Text.Json;
using NRN.Web.Features.Learning.Models;

namespace NRN.Web.Features.Learning.Services;

public sealed class JsonLearningContentService(
    IWebHostEnvironment environment) : ILearningContentService
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Task<IReadOnlyList<LearningCategory>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return ReadListAsync<LearningCategory>(
            "categories.json",
            cancellationToken);
    }

    public Task<IReadOnlyList<LearningTopic>> GetTopicsAsync(
        CancellationToken cancellationToken = default)
    {
        return ReadListAsync<LearningTopic>(
            "topics.json",
            cancellationToken);
    }

    public Task<IReadOnlyList<LearningProduct>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return ReadListAsync<LearningProduct>(
            "products.json",
            cancellationToken);
    }

    public Task<IReadOnlyList<LearningSource>> GetSourcesAsync(
        CancellationToken cancellationToken = default)
    {
        return ReadListAsync<LearningSource>(
            "sources.json",
            cancellationToken);
    }

    public async Task<LearningTopic?> GetTopicBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        var topics = await GetTopicsAsync(cancellationToken);

        return topics.FirstOrDefault(topic =>
            string.Equals(
                topic.Slug,
                slug,
                StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IReadOnlyList<T>> ReadListAsync<T>(
        string fileName,
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(
            environment.ContentRootPath,
            "Features",
            "Learning",
            "Resources",
            fileName);

        await using var stream = File.OpenRead(path);

        return await JsonSerializer.DeserializeAsync<List<T>>(
                   stream,
                   _options,
                   cancellationToken)
               ?? [];
    }
}
