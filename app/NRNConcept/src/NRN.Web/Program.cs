using NRN.Web.Components;
using NRN.Web.Features.Learning;
using NRN.Web.Features.Learning.Services;
using NRN.Web.Features.Simulations;
using NRN.Web.Features.Simulations.Services;
using NRN.Web.Features.SolutionFinder;
using NRN.Web.Shared.State;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AppState>();

builder.Services
    .AddLearningFeature()
    .AddSimulationsFeature()
    .AddSolutionFinderFeature();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

await app.Services.GetRequiredService<ILearningContentValidator>()
    .ValidateAsync();

await app.Services.GetRequiredService<ISimulationContentValidator>()
    .ValidateAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (builder.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}
app.UseAntiforgery();

app.MapStaticAssets();
app.MapGet("/health", () => Results.Ok(new { status = "ok", app = "lab" }));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
