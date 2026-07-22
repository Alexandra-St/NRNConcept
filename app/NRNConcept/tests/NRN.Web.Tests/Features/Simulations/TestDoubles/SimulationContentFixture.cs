using NRN.Web.Features.Simulations.Models;

namespace NRN.Web.Tests.Features.Simulations.TestDoubles;

internal static class SimulationContentFixture
{
    public static StubSimulationContentService CreateValid() => new()
    {
        Simulations = [CreateSimulation()]
    };

    public static Simulation CreateSimulation(
        string slug = "test-simulation") => new(
        slug,
        4,
        SimulationStatus.Available,
        "simulation.title",
        "simulation.cardQuestion",
        "simulation.intro.title",
        "simulation.intro.body",
        "simulation.intro.privacy",
        "simulation.completion",
        [
            new SimulationScene(
                "start",
                1,
                "scene.start.title",
                "scene.start.prompt",
                ["scene.start.content"],
                [
                    new SimulationChoice(
                        "continue",
                        "scene.start.choice.continue",
                        "finish",
                        ["noticed-risk"])
                ]),
            new SimulationScene(
                "finish",
                2,
                "scene.finish.title",
                "scene.finish.prompt",
                ["scene.finish.content"],
                [
                    new SimulationChoice(
                        "complete",
                        "scene.finish.choice.complete",
                        null,
                        ["safe-action"])
                ])
        ],
        [
            new SimulationOutcome(
                "noticed-risk",
                SimulationOutcomeKind.Risk,
                "outcome.risk.title",
                "outcome.risk.explanation",
                "outcome.risk.impact",
                "outcome.risk.alternative"),
            new SimulationOutcome(
                "safe-action",
                SimulationOutcomeKind.SafeAction,
                "outcome.safe.title",
                "outcome.safe.explanation",
                "outcome.safe.impact")
        ],
        new SimulationResultCopy(
            "result.safe",
            "result.risk",
            "result.critical",
            "result.subtitle"),
        [
            new SimulationTakeaway(
                "pause-first",
                "takeaway.title",
                "takeaway.description")
        ],
        new SimulationLink(
            "topic.title",
            "topic.description",
            "/learn/topic"),
        new SimulationLink(
            "product.title",
            "product.description",
            "/finder/product/test"),
        new SimulationLink(
            "exit.title",
            "exit.description",
            "/finder"));
}
