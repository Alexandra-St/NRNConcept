namespace NRN.Web.Features.Simulations.Services;

public interface ISimulationContentValidator
{
    Task ValidateAsync(CancellationToken cancellationToken = default);
}
