using NRN.Web.Features.Simulations.Models;
using NRN.Web.Features.Simulations.Services;
using NRN.Web.Tests.Features.Simulations.TestDoubles;

namespace NRN.Web.Tests.Features.Simulations.ContentValidation;

public sealed class SimulationContentValidatorTests
{
    [Fact]
    public async Task ValidateAsync_AcceptsValidContent()
    {
        var validator = CreateValidator(
            SimulationContentFixture.CreateValid());

        await validator.ValidateAsync(
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ValidateAsync_RejectsDuplicateSlugs()
    {
        var content = SimulationContentFixture.CreateValid();
        var simulation = content.Simulations[0];
        content = content with
        {
            Simulations =
            [
                simulation,
                simulation with { Slug = simulation.Slug.ToUpperInvariant() }
            ]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content).ValidateAsync(
                TestContext.Current.CancellationToken));

        Assert.Contains("Duplicate simulation slug", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsMissingNextScene()
    {
        var content = SimulationContentFixture.CreateValid();
        var simulation = content.Simulations[0];
        var firstScene = simulation.Scenes[0];
        var firstChoice = firstScene.Choices[0];
        simulation = simulation with
        {
            Scenes =
            [
                firstScene with
                {
                    Choices =
                    [
                        firstChoice with { NextSceneId = "missing-scene" }
                    ]
                },
                simulation.Scenes[1]
            ]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content with { Simulations = [simulation] })
                .ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("references missing next scene", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsMissingOutcome()
    {
        var content = SimulationContentFixture.CreateValid();
        var simulation = content.Simulations[0];
        var firstScene = simulation.Scenes[0];
        var firstChoice = firstScene.Choices[0];
        simulation = simulation with
        {
            Scenes =
            [
                firstScene with
                {
                    Choices =
                    [
                        firstChoice with { OutcomeIds = ["missing-outcome"] }
                    ]
                },
                simulation.Scenes[1]
            ]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content with { Simulations = [simulation] })
                .ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("references missing outcome", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsUnreachableScene()
    {
        var content = SimulationContentFixture.CreateValid();
        var simulation = content.Simulations[0];
        var isolatedScene = new SimulationScene(
            "isolated",
            3,
            "scene.isolated.title",
            "scene.isolated.prompt",
            ["scene.isolated.content"],
            [
                new SimulationChoice(
                    "stop",
                    "scene.isolated.choice.stop",
                    null,
                    ["safe-action"])
            ]);
        simulation = simulation with
        {
            Scenes = [.. simulation.Scenes, isolatedScene]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content with { Simulations = [simulation] })
                .ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("is unreachable from the first scene", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsSceneWithoutPathToCompletion()
    {
        var content = SimulationContentFixture.CreateValid();
        var simulation = content.Simulations[0];
        var firstScene = simulation.Scenes[0];
        var firstChoice = firstScene.Choices[0];
        simulation = simulation with
        {
            Scenes =
            [
                firstScene with
                {
                    Choices =
                    [
                        firstChoice with { NextSceneId = firstScene.Id }
                    ]
                },
                simulation.Scenes[1]
            ]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content with { Simulations = [simulation] })
                .ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("has no path to completion", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsMissingLocalization()
    {
        var content = SimulationContentFixture.CreateValid();
        var validator = new SimulationContentValidator(
            content,
            new StubSimulationLocalizationService("simulation.title"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            validator.ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains(
            "Missing localization key 'simulation.title'",
            exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsRiskWithoutAlternative()
    {
        var content = SimulationContentFixture.CreateValid();
        var simulation = content.Simulations[0];
        simulation = simulation with
        {
            Outcomes =
            [
                simulation.Outcomes[0] with { AlternativeKey = null },
                simulation.Outcomes[1]
            ]
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content with { Simulations = [simulation] })
                .ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("must provide a safe alternative", exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_RejectsNonLocalExitUrl()
    {
        var content = SimulationContentFixture.CreateValid();
        var simulation = content.Simulations[0] with
        {
            AlternativeExit = new SimulationLink(
                "exit.title",
                "exit.description",
                "https://example.com")
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CreateValidator(content with { Simulations = [simulation] })
                .ValidateAsync(TestContext.Current.CancellationToken));

        Assert.Contains("must use a local absolute-path URL", exception.Message);
    }

    private static SimulationContentValidator CreateValidator(
        StubSimulationContentService content) =>
        new(content, new StubSimulationLocalizationService());
}
