using NRN.Web.Features.Simulations;
using NRN.Web.Features.Simulations.Models;
using NRN.Web.Features.Simulations.Services;
using NRN.Web.Tests.Features.Simulations.TestDoubles;

namespace NRN.Web.Tests.Features.Simulations;

public sealed class SimulationStateTests
{
    [Fact]
    public void Begin_StartsAtFirstSceneAndClearsPreviousRun()
    {
        var simulation = SimulationContentFixture.CreateSimulation();
        var state = new SimulationState();
        state.Begin(simulation);
        state.SelectChoice("continue");

        state.Begin(simulation);

        Assert.True(state.Started);
        Assert.False(state.Completed);
        Assert.Equal("start", state.CurrentScene?.Id);
        Assert.Empty(state.SelectedChoiceIds);
        Assert.Empty(state.SelectedOutcomeIds);
        Assert.Null(state.ResultKind);
    }

    [Fact]
    public void SelectChoice_RecordsChoiceAndMovesToNextScene()
    {
        var state = new SimulationState();
        state.Begin(SimulationContentFixture.CreateSimulation());

        state.SelectChoice("CONTINUE");

        Assert.Equal("finish", state.CurrentScene?.Id);
        Assert.True(state.IsSelected("start", "continue"));
        Assert.Contains("noticed-risk", state.SelectedOutcomeIds);
        Assert.False(state.Completed);
    }

    [Fact]
    public void SelectChoice_TerminalChoiceCompletesRunAndCalculatesResult()
    {
        var state = new SimulationState();
        state.Begin(SimulationContentFixture.CreateSimulation());
        state.SelectChoice("continue");

        state.SelectChoice("complete");

        Assert.True(state.Completed);
        Assert.Null(state.CurrentScene);
        Assert.Equal(SimulationOutcomeKind.Risk, state.ResultKind);
        Assert.Equal(2, state.GetSelectedOutcomes().Count);
    }

    [Fact]
    public void ResultKind_CriticalRiskTakesPriority()
    {
        var simulation = SimulationContentFixture.CreateSimulation();
        simulation = simulation with
        {
            Outcomes =
            [
                simulation.Outcomes[0] with
                {
                    Kind = SimulationOutcomeKind.CriticalRisk
                },
                simulation.Outcomes[1]
            ]
        };
        var state = new SimulationState();
        state.Begin(simulation);
        state.SelectChoice("continue");
        state.SelectChoice("complete");

        Assert.Equal(
            SimulationOutcomeKind.CriticalRisk,
            state.ResultKind);
    }

    [Fact]
    public void Restart_ClearsProgressAndKeepsSimulation()
    {
        var simulation = SimulationContentFixture.CreateSimulation();
        var state = new SimulationState();
        state.Begin(simulation);
        state.SelectChoice("continue");

        state.Restart();

        Assert.Same(simulation, state.CurrentSimulation);
        Assert.Equal("start", state.CurrentScene?.Id);
        Assert.Empty(state.SelectedChoiceIds);
        Assert.Empty(state.SelectedOutcomeIds);
        Assert.False(state.Completed);
    }

    [Fact]
    public void Reset_RemovesActiveSimulationAndProgress()
    {
        var state = new SimulationState();
        state.Begin(SimulationContentFixture.CreateSimulation());
        state.SelectChoice("continue");

        state.Reset();

        Assert.False(state.Started);
        Assert.False(state.Completed);
        Assert.Null(state.CurrentSimulation);
        Assert.Null(state.CurrentScene);
        Assert.Empty(state.SelectedChoiceIds);
        Assert.Empty(state.SelectedOutcomeIds);
    }

    [Fact]
    public void Begin_RejectsUnavailableSimulation()
    {
        var simulation = SimulationContentFixture.CreateSimulation() with
        {
            Status = SimulationStatus.ComingSoon
        };
        var state = new SimulationState();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            state.Begin(simulation));

        Assert.Contains("is not available", exception.Message);
    }

    [Fact]
    public void SelectChoice_RejectsUnknownChoiceWithoutChangingState()
    {
        var state = new SimulationState();
        state.Begin(SimulationContentFixture.CreateSimulation());

        Assert.Throws<ArgumentException>(() =>
            state.SelectChoice("missing-choice"));

        Assert.Equal("start", state.CurrentScene?.Id);
        Assert.Empty(state.SelectedChoiceIds);
        Assert.Empty(state.SelectedOutcomeIds);
    }

    [Fact]
    public void SelectChoice_RejectsSelectionAfterCompletion()
    {
        var state = new SimulationState();
        state.Begin(SimulationContentFixture.CreateSimulation());
        state.SelectChoice("continue");
        state.SelectChoice("complete");

        var exception = Assert.Throws<InvalidOperationException>(() =>
            state.SelectChoice("complete"));

        Assert.Contains("already been completed", exception.Message);
    }

    [Fact]
    public async Task PublicWifi_SafeFourStepPathCompletesWithoutRiskOutcome()
    {
        var simulation = await LoadPublicWifiAsync();
        var state = new SimulationState();
        state.Begin(simulation);

        state.SelectChoice("verify-network-name");
        state.SelectChoice("continue-as-guest");
        state.SelectChoice("use-mobile-data");
        state.SelectChoice("use-mail-app");

        Assert.True(state.Completed);
        Assert.Equal(SimulationOutcomeKind.SafeAction, state.ResultKind);
        Assert.Equal(4, state.SelectedChoiceIds.Count);
        Assert.DoesNotContain(
            "credentials-exposed",
            state.SelectedOutcomeIds);
    }

    [Fact]
    public async Task PublicWifi_CriticalPathIncludesCredentialExposure()
    {
        var simulation = await LoadPublicWifiAsync();
        var state = new SimulationState();
        state.Begin(simulation);

        state.SelectChoice("strongest-signal");
        state.SelectChoice("primary-contacts");
        state.SelectChoice("ignore-warning");
        state.SelectChoice("enter-credentials");

        Assert.True(state.Completed);
        Assert.Equal(
            SimulationOutcomeKind.CriticalRisk,
            state.ResultKind);
        Assert.Equal(4, state.SelectedChoiceIds.Count);
        Assert.Contains(
            "credentials-exposed",
            state.SelectedOutcomeIds);
    }

    [Fact]
    public async Task OrdinaryDay_FourOrdinaryActionsBuildAProfile()
    {
        var simulation = await LoadSimulationAsync("ordinary-day");
        var state = new SimulationState();
        state.Begin(simulation);

        state.SelectChoice("deny-location");
        state.SelectChoice("open-sushi");
        state.SelectChoice("like-music");
        state.SelectChoice("use-email-alias");

        Assert.True(state.Completed);
        Assert.Equal(4, state.SelectedChoiceIds.Count);
        Assert.Contains("location-approximate", state.SelectedOutcomeIds);
        Assert.Contains("sushi-interest", state.SelectedOutcomeIds);
        Assert.Contains("music-interest", state.SelectedOutcomeIds);
        Assert.Contains("alias-email", state.SelectedOutcomeIds);
    }

    private static Task<Simulation> LoadPublicWifiAsync() =>
        LoadSimulationAsync("public-wifi");

    private static async Task<Simulation> LoadSimulationAsync(string slug)
    {
        var content = new JsonSimulationContentService(
            new TestSimulationWebHostEnvironment(
                AppContext.BaseDirectory));

        return await content.GetBySlugAsync(
                   slug,
                   TestContext.Current.CancellationToken)
               ?? throw new InvalidOperationException(
                   $"The '{slug}' test resource was not found.");
    }
}
