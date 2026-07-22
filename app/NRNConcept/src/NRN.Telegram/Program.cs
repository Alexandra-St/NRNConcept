using NRN.Telegram.Components;
using NRN.Telegram.Features.Services.Api;
using NRN.Telegram.Features.Services.Endpoints;
using NRN.Telegram.Features.Services.Persistence;
using NRN.Telegram.Features.Services.Store;
using NRN.Telegram.Features.Services.State;
using NRN.Telegram.Features.Payments;
using NRN.Telegram.Features.Payments.Api;
using NRN.Telegram.Features.Payments.Endpoints;
using NRN.Telegram.Features.Payments.Store;
using NRN.Telegram.Features.Payments.TelegramBot;
using NRN.Telegram.Telegram;
using Microsoft.EntityFrameworkCore;
using NRN.Telegram.Localization;
using NRN.Telegram.Features.Help;
using NRN.Telegram.Features.Catalog;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDirectory);
var connectionString = builder.Configuration.GetConnectionString("NrnServices")
    ?? $"Data Source={Path.Combine(dataDirectory, "nrn-telegram.db")}";

builder.Services.AddDbContextFactory<NrnServicesDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddSingleton<INrnServicesStore, SqliteNrnServicesStore>();
builder.Services.AddSingleton<ITelegramStarsPaymentStore, SqliteTelegramStarsPaymentStore>();
builder.Services.AddSingleton<IPreviewNrnServicesApi, PreviewNrnServicesApi>();
builder.Services.AddHttpClient<INrnServicesApi, HttpNrnServicesApi>(client =>
    client.Timeout = TimeSpan.FromSeconds(15));
builder.Services.AddHttpClient<INrnPaymentsApi, HttpNrnPaymentsApi>(client =>
    client.Timeout = TimeSpan.FromSeconds(15));
builder.Services.AddHttpClient<ITelegramBotPaymentsClient, TelegramBotPaymentsClient>();
builder.Services.AddScoped<UserServicesState>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(serviceProvider =>
{
    var preferences = new MiniAppPreferencesState();
    var savedLanguage = serviceProvider
        .GetRequiredService<IHttpContextAccessor>()
        .HttpContext?
        .Request
        .Cookies["nrn.language"];
    preferences.Initialize(savedLanguage, null);
    return preferences;
});
builder.Services.AddScoped<MiniAppLocalizer>();
builder.Services
    .AddOptions<TelegramOptions>()
    .Bind(builder.Configuration.GetSection(TelegramOptions.SectionName))
    .Validate(options => SupportLink.IsValidOrEmpty(options.SupportUrl),
        "Telegram:SupportUrl must be an absolute HTTPS URL without credentials.")
    .Validate(options => SupportLink.IsValidOrEmpty(options.FeedbackUrl),
        "Telegram:FeedbackUrl must be an absolute HTTPS URL without credentials.")
    .ValidateOnStart();
builder.Services.Configure<TelegramPaymentsOptions>(
    builder.Configuration.GetSection(TelegramPaymentsOptions.SectionName));
builder.Services.Configure<NarayanaPublicCatalogOptions>(
    builder.Configuration.GetSection(NarayanaPublicCatalogOptions.SectionName));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ITelegramInitDataValidator, TelegramInitDataValidator>();
builder.Services.AddScoped<TelegramSessionState>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost
});

var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

await NrnDatabaseInitializer.InitializeAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

if (builder.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}
app.UseAntiforgery();
app.MapStaticAssets();
app.MapGet("/health", () => Results.Ok(new { status = "ok", app = "miniapp" }));
app.MapNrnServicesApi();
app.MapNrnPaymentsApi();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
