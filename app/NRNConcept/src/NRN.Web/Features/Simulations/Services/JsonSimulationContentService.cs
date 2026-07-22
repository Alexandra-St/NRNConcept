using System.Text.Json;
using NRN.Web.Features.Simulations.Models;

namespace NRN.Web.Features.Simulations.Services;

public sealed class JsonSimulationContentService(
    IWebHostEnvironment environment) : ISimulationContentService
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<Simulation>> GetSimulationsAsync(
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(
            environment.ContentRootPath,
            "Features",
            "Simulations",
            "Resources",
            "simulations.json");

        await using var stream = File.OpenRead(path);

        return await JsonSerializer.DeserializeAsync<List<Simulation>>(
                   stream,
                   _options,
                   cancellationToken)
               ?? [];
    }

    public async Task<Simulation?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        var simulations = await GetSimulationsAsync(cancellationToken);

        return simulations.FirstOrDefault(simulation =>
            string.Equals(
                simulation.Slug,
                slug,
                StringComparison.OrdinalIgnoreCase));
    }
}
