using NRN.Web.Features.Learning.Models;
using NRN.Web.Features.Learning.Services;

namespace NRN.Web.Tests.Features.Learning.TestDoubles;

internal sealed record StubLearningContentService : ILearningContentService
{
    public IReadOnlyList<LearningCategory> Categories { get; init; } = [];
    public IReadOnlyList<LearningTopic> Topics { get; init; } = [];
    public IReadOnlyList<LearningProduct> Products { get; init; } = [];
    public IReadOnlyList<LearningSource> Sources { get; init; } = [];

    public Task<IReadOnlyList<LearningCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Categories);

    public Task<IReadOnlyList<LearningTopic>> GetTopicsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Topics);

    public Task<IReadOnlyList<LearningProduct>> GetProductsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Products);

    public Task<IReadOnlyList<LearningSource>> GetSourcesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Sources);

    public Task<LearningTopic?> GetTopicBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        Task.FromResult(Topics.FirstOrDefault(topic =>
            string.Equals(topic.Slug, slug, StringComparison.OrdinalIgnoreCase)));
}
