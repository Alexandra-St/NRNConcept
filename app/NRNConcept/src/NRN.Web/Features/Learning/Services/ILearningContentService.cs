using NRN.Web.Features.Learning.Models;

namespace NRN.Web.Features.Learning.Services;

public interface ILearningContentService
{
    Task<IReadOnlyList<LearningCategory>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LearningTopic>> GetTopicsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LearningProduct>> GetProductsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LearningSource>> GetSourcesAsync(
        CancellationToken cancellationToken = default);

    Task<LearningTopic?> GetTopicBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);
}
