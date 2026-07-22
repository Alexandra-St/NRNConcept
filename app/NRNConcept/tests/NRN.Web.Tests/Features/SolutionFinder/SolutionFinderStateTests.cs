using NRN.Web.Features.SolutionFinder;
using NRN.Web.Features.SolutionFinder.Models;

namespace NRN.Web.Tests.Features.SolutionFinder;

public sealed class SolutionFinderStateTests
{
    [Fact]
    public void Begin_ClearsAnswersAndStartsNewRun()
    {
        var state = new SolutionFinderState();
        state.Begin();
        state.SelectSingle(FinderQuestionIds.Travel, FinderAnswerIds.TravelOften);
        state.Complete();

        state.Begin();

        Assert.True(state.Started);
        Assert.False(state.Completed);
        Assert.False(state.HasAnswer(FinderQuestionIds.Travel));
    }

    [Fact]
    public void ToggleMultiple_ExclusiveAnswerReplacesOtherAnswers()
    {
        var state = new SolutionFinderState();
        state.Begin();
        state.ToggleMultiple(FinderQuestionIds.NumberUse, FinderAnswerIds.Registrations,
            FinderAnswerIds.CloseCircleOnly);

        state.ToggleMultiple(FinderQuestionIds.NumberUse, FinderAnswerIds.CloseCircleOnly,
            FinderAnswerIds.CloseCircleOnly);

        Assert.True(state.IsSelected(FinderQuestionIds.NumberUse, FinderAnswerIds.CloseCircleOnly));
        Assert.False(state.IsSelected(FinderQuestionIds.NumberUse, FinderAnswerIds.Registrations));
    }
}
