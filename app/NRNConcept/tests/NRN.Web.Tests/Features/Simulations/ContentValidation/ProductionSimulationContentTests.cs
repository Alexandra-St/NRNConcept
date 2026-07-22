using NRN.Web.Features.Simulations.Models;
using NRN.Web.Features.Simulations.Services;
using NRN.Web.Tests.Features.Simulations.TestDoubles;

namespace NRN.Web.Tests.Features.Simulations.ContentValidation;

public sealed class ProductionSimulationContentTests
{
    [Fact]
    public async Task CurrentResources_PassContentValidation()
    {
        var environment = new TestSimulationWebHostEnvironment(
            AppContext.BaseDirectory);
        var content = new JsonSimulationContentService(environment);
        var localization = new JsonSimulationLocalizationService(environment);
        var validator = new SimulationContentValidator(
            content,
            localization);

        await validator.ValidateAsync(
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CurrentResources_ContainBothAvailableSimulations()
    {
        var content = new JsonSimulationContentService(
            new TestSimulationWebHostEnvironment(AppContext.BaseDirectory));

        var simulations = await content.GetSimulationsAsync(
            TestContext.Current.CancellationToken);

        Assert.Equal(2, simulations.Count);
        Assert.All(simulations, simulation =>
            Assert.Equal(SimulationStatus.Available, simulation.Status));

        var publicWifi = Assert.Single(simulations, simulation =>
            simulation.Slug == "public-wifi");
        var ordinaryDay = Assert.Single(simulations, simulation =>
            simulation.Slug == "ordinary-day");

        Assert.Equal(4, publicWifi.Scenes.Count);
        Assert.Equal(4, ordinaryDay.Scenes.Count);
        Assert.Contains(
            publicWifi.Outcomes,
            outcome => outcome.Kind == SimulationOutcomeKind.CriticalRisk);
    }

}
