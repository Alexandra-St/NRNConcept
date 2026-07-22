namespace NRN.Web.Features.Learning.Models;

public sealed record LearningTopic(string Slug,
    string CategoryId,
    int Order,
    string TitleKey,
    string TaglineKey,
    string SummaryKey,
    string SituationKey,
    string ProblemKey,
    string RealityKey,
    string SolutionKey,
    string? RelatedSimulationSlug = null,
    IReadOnlyList<string>? RelatedProductIds = null,
    IReadOnlyList<string>? RelatedSourceIds = null);
