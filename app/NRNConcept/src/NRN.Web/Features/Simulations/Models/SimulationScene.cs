namespace NRN.Web.Features.Simulations.Models;

public sealed record SimulationScene(
    string Id,
    int Order,
    string TitleKey,
    string PromptKey,
    IReadOnlyList<string> ContentKeys,
    IReadOnlyList<SimulationChoice> Choices);

public sealed record SimulationChoice(
    string Id,
    string LabelKey,
    string? NextSceneId,
    IReadOnlyList<string> OutcomeIds);
