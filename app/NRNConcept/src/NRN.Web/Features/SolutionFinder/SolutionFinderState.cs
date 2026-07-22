namespace NRN.Web.Features.SolutionFinder;

public class SolutionFinderState
{
    private readonly Dictionary<string, HashSet<string>> _answers =
        new(StringComparer.OrdinalIgnoreCase);

    public bool Started { get; private set; }
    public bool Completed { get; private set; }

    public void Begin()
    {
        _answers.Clear();
        Started = true;
        Completed = false;
    }

    public void Complete() => Completed = true;

    public void Reset()
    {
        _answers.Clear();
        Started = false;
        Completed = false;
    }

    public IReadOnlyCollection<string> GetAnswers(string questionId) =>
        _answers.TryGetValue(questionId, out var answers) ? answers.ToArray() : [];

    public bool IsSelected(string questionId, string answerId) =>
        _answers.TryGetValue(questionId, out var answers) && answers.Contains(answerId);

    public bool HasAnswer(string questionId) =>
        _answers.TryGetValue(questionId, out var answers) && answers.Count > 0;

    public bool HasAny(string questionId, params string[] answerIds) =>
        _answers.TryGetValue(questionId, out var answers) && answerIds.Any(answers.Contains);

    public void ClearAnswers(string questionId)
    {
        _answers.Remove(questionId);
        Completed = false;
    }

    public void SelectSingle(string questionId, string answerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(questionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(answerId);
        _answers[questionId] = new(StringComparer.OrdinalIgnoreCase) { answerId };
        Completed = false;
    }

    public void ToggleMultiple(string questionId, string answerId, params string[] exclusiveAnswerIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(questionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(answerId);
        if (!_answers.TryGetValue(questionId, out var answers))
        {
            answers = new(StringComparer.OrdinalIgnoreCase);
            _answers[questionId] = answers;
        }

        if (!answers.Add(answerId))
        {
            answers.Remove(answerId);
        }
        else if (exclusiveAnswerIds.Contains(answerId, StringComparer.OrdinalIgnoreCase))
        {
            answers.RemoveWhere(value => !string.Equals(value, answerId, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            foreach (var exclusive in exclusiveAnswerIds) answers.Remove(exclusive);
        }

        Completed = false;
    }
}
