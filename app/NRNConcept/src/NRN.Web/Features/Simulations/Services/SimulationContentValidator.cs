using System.Text.RegularExpressions;
using NRN.Web.Features.Simulations.Models;

namespace NRN.Web.Features.Simulations.Services;

public sealed class SimulationContentValidator(
    ISimulationContentService contentService,
    ISimulationLocalizationService localizationService)
    : ISimulationContentValidator
{
    private static readonly string[] Locales = ["en", "ru"];

    private static readonly string[] InterfaceLocalizationKeys =
    [
        "simulations.home.eyebrow",
        "simulations.home.title",
        "simulations.home.subtitle",
        "simulations.home.privacy.title",
        "simulations.home.privacy.description",
        "simulations.common.available",
        "simulations.common.comingSoon",
        "simulations.common.minutes",
        "simulations.common.start",
        "simulations.common.continue",
        "simulations.common.viewExplanation",
        "simulations.common.restart",
        "simulations.common.backToList",
        "simulations.common.keyTakeaways",
        "simulations.common.relatedTopic",
        "simulations.common.relatedProduct",
        "simulations.common.notFound.title",
        "simulations.common.notFound.description",
        "simulations.common.loading",
        "simulations.common.error.label",
        "simulations.common.error.title",
        "simulations.common.error.message",
        "simulations.common.retry",
        "simulations.common.introEyebrow",
        "simulations.common.progressLabel",
        "simulations.common.step",
        "simulations.common.chooseAction",
        "simulations.results.eyebrow",
        "simulations.results.yourDecisions",
        "simulations.results.whyImportant",
        "simulations.results.whatWorked",
        "simulations.results.alternative",
        "simulations.results.safeDecision",
        "simulations.results.risk",
        "simulations.results.criticalRisk",
        "simulations.results.related",
        "simulations.results.restart",
        "simulations.results.otherSimulations",
        "simulations.results.openTopic",
        "simulations.results.openProduct",
        "simulations.results.openFinder"
    ];

    private static readonly Regex SlugPattern = new(
        "^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.CultureInvariant);

    public async Task ValidateAsync(
        CancellationToken cancellationToken = default)
    {
        var simulations = await contentService.GetSimulationsAsync(
            cancellationToken);
        var errors = new List<string>();

        if (simulations.Count == 0)
        {
            errors.Add("Simulation content must contain at least one simulation.");
        }

        if (!simulations.Any(simulation =>
                simulation.Status == SimulationStatus.Available))
        {
            errors.Add("Simulation content must contain at least one available simulation.");
        }

        ValidateUnique(
            simulations.Select(simulation => simulation.Slug),
            "simulation slug",
            errors);

        foreach (var simulation in simulations)
        {
            ValidateSimulation(simulation, errors);
        }

        var localizationKeys = InterfaceLocalizationKeys
            .Concat(simulations.SelectMany(GetLocalizationKeys))
            .Distinct(StringComparer.Ordinal);

        foreach (var locale in Locales)
        foreach (var key in localizationKeys)
        {
            var value = await localizationService.GetAsync(
                key,
                locale,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(value) ||
                string.Equals(value, key, StringComparison.Ordinal))
            {
                errors.Add(
                    $"Missing localization key '{key}' for locale '{locale}'.");
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Simulation content validation failed:\n- " +
                string.Join("\n- ", errors));
        }
    }

    private static void ValidateSimulation(
        Simulation simulation,
        ICollection<string> errors)
    {
        if (!SlugPattern.IsMatch(simulation.Slug))
        {
            errors.Add(
                $"Simulation slug '{simulation.Slug}' must use lowercase kebab-case.");
        }

        if (simulation.DurationMinutes <= 0)
        {
            errors.Add(
                $"Simulation '{simulation.Slug}' must have a positive duration.");
        }

        if (simulation.Scenes.Count == 0)
        {
            errors.Add(
                $"Simulation '{simulation.Slug}' must contain at least one scene.");
            return;
        }

        ValidateUnique(
            simulation.Scenes.Select(scene => scene.Id),
            $"scene id in simulation '{simulation.Slug}'",
            errors);
        ValidateUnique(
            simulation.Scenes.Select(scene => scene.Order.ToString()),
            $"scene order in simulation '{simulation.Slug}'",
            errors);
        ValidateUnique(
            simulation.Outcomes.Select(outcome => outcome.Id),
            $"outcome id in simulation '{simulation.Slug}'",
            errors);
        ValidateUnique(
            simulation.KeyTakeaways.Select(takeaway => takeaway.Id),
            $"takeaway id in simulation '{simulation.Slug}'",
            errors);

        ValidateSceneOrder(simulation, errors);
        ValidateChoicesAndReferences(simulation, errors);
        ValidateReachability(simulation, errors);
        ValidateLinks(simulation, errors);
    }

    private static void ValidateSceneOrder(
        Simulation simulation,
        ICollection<string> errors)
    {
        var actual = simulation.Scenes
            .Select(scene => scene.Order)
            .Order()
            .ToArray();
        var expected = Enumerable
            .Range(1, simulation.Scenes.Count)
            .ToArray();

        if (!actual.SequenceEqual(expected))
        {
            errors.Add(
                $"Simulation '{simulation.Slug}' scene order must be contiguous and start at 1.");
        }
    }

    private static void ValidateChoicesAndReferences(
        Simulation simulation,
        ICollection<string> errors)
    {
        var sceneIds = simulation.Scenes
            .Select(scene => scene.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var outcomeIds = simulation.Outcomes
            .Select(outcome => outcome.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var referencedOutcomeIds = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);
        var hasTerminalChoice = false;

        foreach (var scene in simulation.Scenes)
        {
            ValidateIdentifier(scene.Id, "scene", simulation.Slug, errors);

            ValidateUnique(
                scene.Choices.Select(choice => choice.Id),
                $"choice id in scene '{scene.Id}'",
                errors);

            if (scene.Choices.Count == 0)
            {
                errors.Add(
                    $"Scene '{scene.Id}' in simulation '{simulation.Slug}' must contain at least one choice.");
            }

            foreach (var choice in scene.Choices)
            {
                ValidateIdentifier(
                    choice.Id,
                    $"choice in scene '{scene.Id}'",
                    simulation.Slug,
                    errors);

                if (choice.NextSceneId is null)
                {
                    hasTerminalChoice = true;
                }
                else if (!sceneIds.Contains(choice.NextSceneId))
                {
                    errors.Add(
                        $"Choice '{choice.Id}' in scene '{scene.Id}' references missing next scene '{choice.NextSceneId}'.");
                }

                foreach (var outcomeId in choice.OutcomeIds)
                {
                    referencedOutcomeIds.Add(outcomeId);

                    if (!outcomeIds.Contains(outcomeId))
                    {
                        errors.Add(
                            $"Choice '{choice.Id}' in scene '{scene.Id}' references missing outcome '{outcomeId}'.");
                    }
                }
            }
        }

        foreach (var outcome in simulation.Outcomes)
        {
            ValidateIdentifier(outcome.Id, "outcome", simulation.Slug, errors);

            if (outcome.Kind != SimulationOutcomeKind.SafeAction &&
                string.IsNullOrWhiteSpace(outcome.AlternativeKey))
            {
                errors.Add(
                    $"Outcome '{outcome.Id}' in simulation '{simulation.Slug}' must provide a safe alternative.");
            }
        }

        foreach (var takeaway in simulation.KeyTakeaways)
        {
            ValidateIdentifier(takeaway.Id, "takeaway", simulation.Slug, errors);
        }

        if (!hasTerminalChoice)
        {
            errors.Add(
                $"Simulation '{simulation.Slug}' must contain at least one choice that completes the story.");
        }

        foreach (var outcomeId in outcomeIds.Except(
                     referencedOutcomeIds,
                     StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(
                $"Outcome '{outcomeId}' in simulation '{simulation.Slug}' is never used by a choice.");
        }
    }

    private static void ValidateReachability(
        Simulation simulation,
        ICollection<string> errors)
    {
        var scenesById = simulation.Scenes.ToDictionary(
            scene => scene.Id,
            StringComparer.OrdinalIgnoreCase);
        var firstScene = simulation.Scenes.MinBy(scene => scene.Order)!;
        var reachable = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<string>();
        queue.Enqueue(firstScene.Id);

        while (queue.TryDequeue(out var sceneId))
        {
            if (!reachable.Add(sceneId) || !scenesById.TryGetValue(sceneId, out var scene))
            {
                continue;
            }

            foreach (var nextSceneId in scene.Choices
                         .Select(choice => choice.NextSceneId)
                         .Where(next => next is not null))
            {
                queue.Enqueue(nextSceneId!);
            }
        }

        foreach (var scene in simulation.Scenes.Where(
                     scene => !reachable.Contains(scene.Id)))
        {
            errors.Add(
                $"Scene '{scene.Id}' in simulation '{simulation.Slug}' is unreachable from the first scene.");
        }

        var reverseEdges = simulation.Scenes.ToDictionary(
            scene => scene.Id,
            _ => new List<string>(),
            StringComparer.OrdinalIgnoreCase);

        foreach (var scene in simulation.Scenes)
        foreach (var nextSceneId in scene.Choices
                     .Select(choice => choice.NextSceneId)
                     .Where(next => next is not null && scenesById.ContainsKey(next)))
        {
            reverseEdges[nextSceneId!].Add(scene.Id);
        }

        var canComplete = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var completionQueue = new Queue<string>(
            simulation.Scenes
                .Where(scene => scene.Choices.Any(choice => choice.NextSceneId is null))
                .Select(scene => scene.Id));

        while (completionQueue.TryDequeue(out var sceneId))
        {
            if (!canComplete.Add(sceneId))
            {
                continue;
            }

            foreach (var previousSceneId in reverseEdges[sceneId])
            {
                completionQueue.Enqueue(previousSceneId);
            }
        }

        foreach (var scene in simulation.Scenes.Where(
                     scene => !canComplete.Contains(scene.Id)))
        {
            errors.Add(
                $"Scene '{scene.Id}' in simulation '{simulation.Slug}' has no path to completion.");
        }
    }

    private static void ValidateLinks(
        Simulation simulation,
        ICollection<string> errors)
    {
        ValidateLink(simulation.Slug, "related topic", simulation.RelatedTopic, errors);

        if (simulation.RelatedProduct is not null)
        {
            ValidateLink(simulation.Slug, "related product", simulation.RelatedProduct, errors);
        }

        if (simulation.AlternativeExit is not null)
        {
            ValidateLink(simulation.Slug, "alternative exit", simulation.AlternativeExit, errors);
        }
    }

    private static void ValidateLink(
        string simulationSlug,
        string label,
        SimulationLink link,
        ICollection<string> errors)
    {
        if (!link.Url.StartsWith('/') || link.Url.StartsWith("//"))
        {
            errors.Add(
                $"Simulation '{simulationSlug}' {label} must use a local absolute-path URL.");
        }
    }

    private static void ValidateIdentifier(
        string id,
        string label,
        string simulationSlug,
        ICollection<string> errors)
    {
        if (!SlugPattern.IsMatch(id))
        {
            errors.Add(
                $"The {label} id '{id}' in simulation '{simulationSlug}' must use lowercase kebab-case.");
        }
    }

    private static IEnumerable<string> GetLocalizationKeys(
        Simulation simulation)
    {
        IEnumerable<string> keys =
        [
            simulation.TitleKey,
            simulation.CardQuestionKey,
            simulation.IntroTitleKey,
            simulation.IntroBodyKey,
            simulation.PrivacyNoteKey,
            simulation.CompletionKey,
            simulation.Result.SafeTitleKey,
            simulation.Result.RiskTitleKey,
            simulation.Result.CriticalTitleKey,
            simulation.Result.SubtitleKey,
            simulation.RelatedTopic.TitleKey,
            simulation.RelatedTopic.DescriptionKey
        ];

        keys = keys
            .Concat(simulation.Scenes.SelectMany(scene =>
                new[] { scene.TitleKey, scene.PromptKey }
                    .Concat(scene.ContentKeys)
                    .Concat(scene.Choices.Select(choice => choice.LabelKey))))
            .Concat(simulation.Outcomes.SelectMany(outcome =>
                new[]
                {
                    outcome.TitleKey,
                    outcome.ExplanationKey,
                    outcome.ImpactKey
                }.Concat(outcome.AlternativeKey is { } alternativeKey
                    ? [alternativeKey]
                    : [])))
            .Concat(simulation.KeyTakeaways.SelectMany(takeaway =>
                new[] { takeaway.TitleKey, takeaway.DescriptionKey }));

        if (simulation.RelatedProduct is not null)
        {
            keys = keys.Concat(
                [
                    simulation.RelatedProduct.TitleKey,
                    simulation.RelatedProduct.DescriptionKey
                ]);
        }

        if (simulation.AlternativeExit is not null)
        {
            keys = keys.Concat(
                [
                    simulation.AlternativeExit.TitleKey,
                    simulation.AlternativeExit.DescriptionKey
                ]);
        }

        keys = keys.Concat(simulation.UiLocalizationKeys ?? []);

        return keys;
    }

    private static void ValidateUnique(
        IEnumerable<string> values,
        string label,
        ICollection<string> errors)
    {
        foreach (var duplicate in values
                     .GroupBy(value => value, StringComparer.OrdinalIgnoreCase)
                     .Where(group => group.Count() > 1))
        {
            errors.Add($"Duplicate {label}: '{duplicate.Key}'.");
        }
    }
}
