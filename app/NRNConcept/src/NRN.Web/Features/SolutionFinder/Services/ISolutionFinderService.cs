using NRN.Web.Features.SolutionFinder.Models;
using NRN.Web.Features.SolutionFinder;

namespace NRN.Web.Features.SolutionFinder.Services;

public interface ISolutionFinderService
{
    IReadOnlyList<FinderQuestion> GetQuestions(string locale, SolutionFinderState state);
    FinderResult Evaluate(string locale, SolutionFinderState state);
    FinderProduct? GetProduct(string productId, string locale);
}
