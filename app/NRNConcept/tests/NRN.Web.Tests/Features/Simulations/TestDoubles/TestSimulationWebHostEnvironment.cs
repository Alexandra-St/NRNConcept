using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace NRN.Web.Tests.Features.Simulations.TestDoubles;

internal sealed class TestSimulationWebHostEnvironment(
    string contentRootPath) : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "NRN.Web.Tests";
    public IFileProvider WebRootFileProvider { get; set; } =
        new NullFileProvider();
    public string WebRootPath { get; set; } = contentRootPath;
    public string EnvironmentName { get; set; } = "Test";
    public string ContentRootPath { get; set; } = contentRootPath;
    public IFileProvider ContentRootFileProvider { get; set; } =
        new PhysicalFileProvider(contentRootPath);
}
