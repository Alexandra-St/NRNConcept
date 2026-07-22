using System.Text.Json.Serialization;

namespace NRN.Web.Features.Simulations.Models;

public sealed record Simulation(
    string Slug,
    int DurationMinutes,
    SimulationStatus Status,
    string TitleKey,
    string CardQuestionKey,
    string IntroTitleKey,
    string IntroBodyKey,
    string PrivacyNoteKey,
    string CompletionKey,
    IReadOnlyList<SimulationScene> Scenes,
    IReadOnlyList<SimulationOutcome> Outcomes,
    SimulationResultCopy Result,
    IReadOnlyList<SimulationTakeaway> KeyTakeaways,
    SimulationLink RelatedTopic,
    SimulationLink? RelatedProduct = null,
    SimulationLink? AlternativeExit = null,
    IReadOnlyList<string>? UiLocalizationKeys = null);

[JsonConverter(typeof(JsonStringEnumConverter<SimulationStatus>))]
public enum SimulationStatus
{
    Available,
    ComingSoon
}
