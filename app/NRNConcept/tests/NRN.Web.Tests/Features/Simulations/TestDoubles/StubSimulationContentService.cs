using NRN.Web.Features.Simulations.Models;
using NRN.Web.Features.Simulations.Services;

namespace NRN.Web.Tests.Features.Simulations.TestDoubles;

internal sealed record StubSimulationContentService
    : ISimulationContentService
{
    public IReadOnlyList<Simulation> Simulations { get; init; } = [];

    public Task<IReadOnlyList<Simulation>> GetSimulationsAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Simulations);

    public Task<Simulation?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Simulations.FirstOrDefault(simulation =>
            string.Equals(
                simulation.Slug,
                slug,
                StringComparison.OrdinalIgnoreCase)));
}
