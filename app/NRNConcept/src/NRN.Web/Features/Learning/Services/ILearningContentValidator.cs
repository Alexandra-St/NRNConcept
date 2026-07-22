namespace NRN.Web.Features.Learning.Services;

public interface ILearningContentValidator
{
    Task ValidateAsync(CancellationToken cancellationToken = default);
}
