using NRN.Web.Features.Simulations.Models;

namespace NRN.Web.Features.Simulations.Services;

public interface ISimulationContentService
{
    Task<IReadOnlyList<Simulation>> GetSimulationsAsync(
        CancellationToken cancellationToken = default);

    Task<Simulation?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);
}
