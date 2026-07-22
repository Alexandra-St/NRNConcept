using NRN.Web.Features.Learning.Models;

namespace NRN.Web.Tests.Features.Learning.TestDoubles;

internal static class LearningContentFixture
{
    public static StubLearningContentService CreateValid() => new()
    {
        Categories = [new LearningCategory("foundations", "category.name", "category.description")],
        Topics = [CreateTopic("privacy", 1)],
        Products = [new LearningProduct("virtual-number", "product.name", "product.benefit", "product.limitation")],
        Sources = [new LearningSource("nist", "NIST", "Guidance", "https://example.com/guidance")]
    };

    public static LearningTopic CreateTopic(
        string slug,
        int order,
        string categoryId = "foundations",
        IReadOnlyList<string>? productIds = null,
        IReadOnlyList<string>? sourceIds = null) => new(
        slug,
        categoryId,
        order,
        $"topic.{slug}.title",
        $"topic.{slug}.tagline",
        $"topic.{slug}.summary",
        $"topic.{slug}.situation",
        $"topic.{slug}.problem",
        $"topic.{slug}.reality",
        $"topic.{slug}.solution",
        RelatedProductIds: productIds ?? ["virtual-number"],
        RelatedSourceIds: sourceIds ?? ["nist"]);
}
