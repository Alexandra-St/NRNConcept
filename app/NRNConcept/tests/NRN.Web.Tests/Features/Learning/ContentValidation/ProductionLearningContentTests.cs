using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using NRN.Web.Features.Learning.Services;

namespace NRN.Web.Tests.Features.Learning.ContentValidation;

public sealed class ProductionLearningContentTests
{
    [Fact]
    public async Task CurrentResources_PassContentValidation()
    {
        var environment = new TestWebHostEnvironment(AppContext.BaseDirectory);
        var content = new JsonLearningContentService(environment);
        var localization = new JsonLearningLocalizationService(environment);
        var validator = new LearningContentValidator(content, localization);

        await validator.ValidateAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CurrentResources_ContainExpectedMvpContent()
    {
        var content = new JsonLearningContentService(new TestWebHostEnvironment(AppContext.BaseDirectory));

        var topics = await content.GetTopicsAsync(TestContext.Current.CancellationToken);
        var products = await content.GetProductsAsync(TestContext.Current.CancellationToken);

        Assert.Equal(10, topics.Count);
        Assert.All(topics, topic => Assert.NotEmpty(topic.RelatedSourceIds ?? []));
        Assert.DoesNotContain(products, product => string.Equals(product.Id, "vpn", StringComparison.OrdinalIgnoreCase));
        Assert.All(topics, topic => Assert.DoesNotContain(topic.RelatedProductIds ?? [],
            productId => string.Equals(productId, "vpn", StringComparison.OrdinalIgnoreCase)));
    }

    private sealed class TestWebHostEnvironment(string contentRootPath) : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "NRN.Web.Tests";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = contentRootPath;
        public string EnvironmentName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = contentRootPath;
        public IFileProvider ContentRootFileProvider { get; set; } = new PhysicalFileProvider(contentRootPath);
    }
}
