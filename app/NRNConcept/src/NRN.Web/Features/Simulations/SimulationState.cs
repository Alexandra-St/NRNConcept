using NRN.Web.Features.Simulations.Models;

namespace NRN.Web.Features.Simulations;

public sealed class SimulationState
{
    private readonly Dictionary<string, string> _selectedChoiceIds =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _selectedOutcomeIds =
        new(StringComparer.OrdinalIgnoreCase);

    public bool Started { get; private set; }
    public bool Completed { get; private set; }
    public Simulation? CurrentSimulation { get; private set; }
    public SimulationScene? CurrentScene { get; private set; }

    public IReadOnlyDictionary<string, string> SelectedChoiceIds =>
        new Dictionary<string, string>(
            _selectedChoiceIds,
            StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> SelectedOutcomeIds =>
        _selectedOutcomeIds.ToArray();

    public SimulationOutcomeKind? ResultKind
    {
        get
        {
            if (!Completed)
            {
                return null;
            }

            var outcomes = GetSelectedOutcomes();

            if (outcomes.Any(outcome =>
                    outcome.Kind == SimulationOutcomeKind.CriticalRisk))
            {
                return SimulationOutcomeKind.CriticalRisk;
            }

            return outcomes.Any(outcome =>
                outcome.Kind == SimulationOutcomeKind.Risk)
                ? SimulationOutcomeKind.Risk
                : SimulationOutcomeKind.SafeAction;
        }
    }

    public void Begin(Simulation simulation)
    {
        ArgumentNullException.ThrowIfNull(simulation);

        if (simulation.Status != SimulationStatus.Available)
        {
            throw new InvalidOperationException(
                $"Simulation '{simulation.Slug}' is not available.");
        }

        var firstScene = simulation.Scenes.MinBy(scene => scene.Order)
            ?? throw new InvalidOperationException(
                $"Simulation '{simulation.Slug}' does not contain any scenes.");

        _selectedChoiceIds.Clear();
        _selectedOutcomeIds.Clear();
        CurrentSimulation = simulation;
        CurrentScene = firstScene;
        Started = true;
        Completed = false;
    }

    public void SelectChoice(string choiceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(choiceId);

        if (!Started)
        {
            throw new InvalidOperationException(
                "A simulation must be started before selecting a choice.");
        }

        if (Completed)
        {
            throw new InvalidOperationException(
                "The simulation has already been completed.");
        }

        var simulation = CurrentSimulation
            ?? throw new InvalidOperationException(
                "The simulation does not have active content.");
        var currentScene = CurrentScene
            ?? throw new InvalidOperationException(
                "The simulation does not have an active scene.");

        var choice = currentScene.Choices.FirstOrDefault(item =>
            string.Equals(
                item.Id,
                choiceId,
                StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException(
                $"Choice '{choiceId}' does not exist in scene '{currentScene.Id}'.",
                nameof(choiceId));

        SimulationScene? nextScene = null;

        if (choice.NextSceneId is not null)
        {
            nextScene = simulation.Scenes.FirstOrDefault(scene =>
                string.Equals(
                    scene.Id,
                    choice.NextSceneId,
                    StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException(
                    $"Choice '{choice.Id}' references missing scene '{choice.NextSceneId}'.");
        }

        var knownOutcomeIds = simulation.Outcomes
            .Select(outcome => outcome.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingOutcomeId = choice.OutcomeIds.FirstOrDefault(
            outcomeId => !knownOutcomeIds.Contains(outcomeId));

        if (missingOutcomeId is not null)
        {
            throw new InvalidOperationException(
                $"Choice '{choice.Id}' references missing outcome '{missingOutcomeId}'.");
        }

        _selectedChoiceIds[currentScene.Id] = choice.Id;

        foreach (var outcomeId in choice.OutcomeIds)
        {
            _selectedOutcomeIds.Add(outcomeId);
        }

        if (nextScene is null)
        {
            CurrentScene = null;
            Completed = true;
            return;
        }

        CurrentScene = nextScene;
    }

    public bool IsSelected(string sceneId, string choiceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sceneId);
        ArgumentException.ThrowIfNullOrWhiteSpace(choiceId);

        return _selectedChoiceIds.TryGetValue(sceneId, out var selected) &&
               string.Equals(
                   selected,
                   choiceId,
                   StringComparison.OrdinalIgnoreCase);
    }

    public IReadOnlyList<SimulationOutcome> GetSelectedOutcomes()
    {
        if (CurrentSimulation is null)
        {
            return [];
        }

        return CurrentSimulation.Outcomes
            .Where(outcome => _selectedOutcomeIds.Contains(outcome.Id))
            .ToArray();
    }

    public void Restart()
    {
        var simulation = CurrentSimulation
            ?? throw new InvalidOperationException(
                "A simulation must be started before it can be restarted.");

        Begin(simulation);
    }

    public void Reset()
    {
        _selectedChoiceIds.Clear();
        _selectedOutcomeIds.Clear();
        CurrentSimulation = null;
        CurrentScene = null;
        Started = false;
        Completed = false;
    }
}
