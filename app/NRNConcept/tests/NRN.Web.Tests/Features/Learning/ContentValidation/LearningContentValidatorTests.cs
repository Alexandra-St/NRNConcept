using NRN.Web.Features.Learning.Models;
using NRN.Web.Features.Learning.Services;
using NRN.Web.Tests.Features.Learning.TestDoubles;

namespace NRN.Web.Tests.Features.Learning.ContentValidation;

public sealed class LearningContentValidatorTests
{
    [Fact]
    public async Task ValidateAsync_AcceptsValidContent()
    {
        var validator = CreateValidator(LearningContentFixture.CreateValid());

        await validator.ValidateAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ValidateAsync_RejectsDuplicateTopicSlugs()
    {
        var content = LearningContentFixture.CreateValid() with
        {
            Topics = [LearningContentFixture.CreateTopic("privacy", 1), LearningContentFixture.CreateTopic("PRIVACY", 2)]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content).ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("Duplicate topic slug", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsMissingCategory()
    {
        var content = LearningContentFixture.CreateValid() with
        {
            Topics = [LearningContentFixture.CreateTopic("privacy", 1, categoryId: "missing")]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content).ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("missing category", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsMissingProduct()
    {
        var content = LearningContentFixture.CreateValid() with
        {
            Topics = [LearningContentFixture.CreateTopic("privacy", 1, productIds: ["missing"])]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content).ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("missing product", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsMissingSource()
    {
        var content = LearningContentFixture.CreateValid() with
        {
            Topics = [LearningContentFixture.CreateTopic("privacy", 1, sourceIds: ["missing"])]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content).ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("missing source", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsDuplicateOrderWithinCategory()
    {
        var content = LearningContentFixture.CreateValid() with
        {
            Topics = [LearningContentFixture.CreateTopic("privacy", 1), LearningContentFixture.CreateTopic("footprint", 1)]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content).ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("Duplicate topic order", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsNonHttpsSourceUrl()
    {
        var content = LearningContentFixture.CreateValid() with
        {
            Sources = [new LearningSource("nist", "NIST", "Guidance", "http://example.com/guidance")]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content).ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("valid HTTPS URL", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsMissingLocalization()
    {
        var content = LearningContentFixture.CreateValid();
        var validator = new LearningContentValidator(content, new StubLearningLocalizationService("category.name"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validator.ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("Missing localization key 'category.name'", exception.Message);
    }

    private static LearningContentValidator CreateValidator(StubLearningContentService content) =>
        new(content, new StubLearningLocalizationService());
}
