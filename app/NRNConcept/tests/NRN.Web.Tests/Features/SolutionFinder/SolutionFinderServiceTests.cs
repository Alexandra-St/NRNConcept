using NRN.Web.Features.SolutionFinder;
using NRN.Web.Features.SolutionFinder.Models;
using NRN.Web.Features.SolutionFinder.Services;

namespace NRN.Web.Tests.Features.SolutionFinder;

public sealed class SolutionFinderServiceTests
{
    private readonly SolutionFinderService _service = new();

    [Fact]
    public void GetQuestions_AddsRelevantFollowUps()
    {
        var state = StartedState();
        state.ToggleMultiple(FinderQuestionIds.NumberUse, FinderAnswerIds.Registrations);
        state.SelectSingle(FinderQuestionIds.Travel, FinderAnswerIds.TravelOften);

        var questions = _service.GetQuestions("ru", state);

        Assert.Contains(questions, question => question.Id == FinderQuestionIds.NumberPurpose);
        Assert.Contains(questions, question => question.Id == FinderQuestionIds.TravelChallenge);
    }

    [Fact]
    public void GetQuestions_IncludesNeutralNumberAnswerAndNoEmptyPriorityChoice()
    {
        var questions = _service.GetQuestions("en", StartedState());

        var numberUse = Assert.Single(questions, question => question.Id == FinderQuestionIds.NumberUse);
        Assert.Contains(numberUse.Options, option => option.Id == FinderAnswerIds.AvoidSharing);

        var priority = Assert.Single(questions, question => question.Id == FinderQuestionIds.Priority);
        Assert.Equal(4, priority.Options.Count);
    }

    [Fact]
    public void Evaluate_RegistrationUseRecommendsVirtualNumberWithHighPriority()
    {
        var state = StartedState();
        state.ToggleMultiple(FinderQuestionIds.NumberUse, FinderAnswerIds.Registrations);

        var result = _service.Evaluate("ru", state);

        var recommendation = Assert.Single(result.Recommendations);
        Assert.Equal("virtual-number", recommendation.Product.Id);
        Assert.True(recommendation.IsHighPriority);
    }

    [Fact]
    public void Evaluate_PublicWifiGoalDoesNotRecommendAProduct()
    {
        var state = StartedState();
        state.ToggleMultiple(FinderQuestionIds.Goals, FinderAnswerIds.SaferInternet);

        var result = _service.Evaluate("en", state);

        Assert.Empty(result.Recommendations);
        Assert.Contains(result.LearningLinks, link => link.Url.EndsWith("/public-wifi", StringComparison.Ordinal));
    }

    [Fact]
    public void GetProduct_DoesNotExposeVpnAsNarayanaProduct()
    {
        Assert.Null(_service.GetProduct("vpn", "en"));
        Assert.NotNull(_service.GetProduct("virtual-number", "en"));
        Assert.NotNull(_service.GetProduct("esim", "en"));
        Assert.NotNull(_service.GetProduct("sip", "en"));
    }

    [Fact]
    public void Evaluate_FrequentTravelWithMobileInternetNeedRecommendsEsimWithHighPriority()
    {
        var state = StartedState();
        state.SelectSingle(FinderQuestionIds.Travel, FinderAnswerIds.TravelOften);
        state.SelectSingle(FinderQuestionIds.TravelChallenge, FinderAnswerIds.MobileInternet);

        var result = _service.Evaluate("en", state);

        var recommendation = Assert.Single(result.Recommendations);
        Assert.Equal("esim", recommendation.Product.Id);
        Assert.True(recommendation.IsHighPriority);
    }

    [Fact]
    public void Evaluate_OccasionalTravelWithMobileInternetNeedRecommendsEsimWithNormalPriority()
    {
        var state = StartedState();
        state.SelectSingle(FinderQuestionIds.Travel, FinderAnswerIds.TravelSometimes);
        state.SelectSingle(FinderQuestionIds.TravelChallenge, FinderAnswerIds.MobileInternet);

        var result = _service.Evaluate("en", state);

        var recommendation = Assert.Single(result.Recommendations);
        Assert.Equal("esim", recommendation.Product.Id);
        Assert.False(recommendation.IsHighPriority);
    }

    [Theory]
    [InlineData(FinderAnswerIds.PublicWifi)]
    [InlineData(FinderAnswerIds.KeepNumber)]
    [InlineData(FinderAnswerIds.NoTravelChallenge)]
    public void Evaluate_TravelWithoutMobileInternetNeedDoesNotRecommendEsim(string travelChallenge)
    {
        var state = StartedState();
        state.ToggleMultiple(FinderQuestionIds.Goals, FinderAnswerIds.TravelConnectivity);
        state.SelectSingle(FinderQuestionIds.Travel, FinderAnswerIds.TravelOften);
        state.SelectSingle(FinderQuestionIds.TravelChallenge, travelChallenge);

        var result = _service.Evaluate("en", state);

        Assert.DoesNotContain(result.Recommendations, item => item.Product.Id == "esim");
    }

    [Fact]
    public void Evaluate_AvoidingNumberSharingDoesNotRecommendVirtualNumber()
    {
        var state = StartedState();
        state.ToggleMultiple(FinderQuestionIds.NumberUse, FinderAnswerIds.AvoidSharing);

        var result = _service.Evaluate("en", state);

        Assert.DoesNotContain(result.Recommendations, item => item.Product.Id == "virtual-number");
    }

    [Fact]
    public void Evaluate_InternetCallsRecommendSip()
    {
        var state = StartedState();
        state.ToggleMultiple(FinderQuestionIds.Calls, FinderAnswerIds.InternetCalls);

        var result = _service.Evaluate("ru", state);

        var recommendation = Assert.Single(result.Recommendations);
        Assert.Equal("sip", recommendation.Product.Id);
        Assert.True(recommendation.IsHighPriority);
    }

    [Theory]
    [InlineData(FinderAnswerIds.Privacy, "complete call anonymity")]
    [InlineData(FinderAnswerIds.Simplicity, "not the simplest option")]
    [InlineData(FinderAnswerIds.International, "priority you selected")]
    [InlineData(FinderAnswerIds.Separation, "does not replace a separate number")]
    public void Evaluate_EveryPriorityChangesSipExplanation(string priority, string expectedText)
    {
        var state = StartedState();
        state.ToggleMultiple(FinderQuestionIds.Calls, FinderAnswerIds.InternetCalls);
        state.SelectSingle(FinderQuestionIds.Priority, priority);

        var result = _service.Evaluate("en", state);

        var recommendation = Assert.Single(result.Recommendations);
        Assert.Contains(expectedText, recommendation.Why, StringComparison.Ordinal);
    }

    private static SolutionFinderState StartedState()
    {
        var state = new SolutionFinderState();
        state.Begin();
        return state;
    }
}
