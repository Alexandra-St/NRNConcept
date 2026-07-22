using System.Text.Json.Serialization;

namespace NRN.Web.Features.Simulations.Models;

public sealed record SimulationOutcome(
    string Id,
    SimulationOutcomeKind Kind,
    string TitleKey,
    string ExplanationKey,
    string ImpactKey,
    string? AlternativeKey = null);

[JsonConverter(typeof(JsonStringEnumConverter<SimulationOutcomeKind>))]
public enum SimulationOutcomeKind
{
    SafeAction,
    Risk,
    CriticalRisk
}

public sealed record SimulationTakeaway(
    string Id,
    string TitleKey,
    string DescriptionKey);

public sealed record SimulationResultCopy(
    string SafeTitleKey,
    string RiskTitleKey,
    string CriticalTitleKey,
    string SubtitleKey);
