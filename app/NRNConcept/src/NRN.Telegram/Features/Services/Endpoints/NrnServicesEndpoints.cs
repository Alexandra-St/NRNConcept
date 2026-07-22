using NRN.Telegram.Features.Services.Api;
using NRN.Telegram.Features.Services.Store;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Features.Services.Endpoints;

public static class NrnServicesEndpoints
{
    private const int MaxInitDataLength = 16 * 1024;

    public static void MapNrnServicesApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1")
            .WithTags("NRN Services API");

        group.MapGet("/catalog", GetCatalogAsync)
            .WithName("GetServiceCatalog");
        group.MapGet("/catalog/{productId}", GetProductAsync)
            .WithName("GetServiceProduct");
        group.MapGet("/services", GetUserServicesAsync)
            .WithName("GetCurrentUserServices");
        group.MapPost("/services/{productId}", PurchaseAsync)
            .WithName("PurchaseService");
    }

    private static async Task<IResult> GetProductAsync(
        string productId,
        INrnServicesStore store,
        CancellationToken cancellationToken)
    {
        var product = await store.GetProductAsync(productId, cancellationToken);
        return product is null ? Results.NotFound() : Results.Ok(product);
    }

    private static async Task<IResult> GetCatalogAsync(
        INrnServicesStore store,
        CancellationToken cancellationToken)
    {
        var catalog = await store.GetCatalogAsync(cancellationToken);
        return Results.Ok(catalog);
    }

    private static async Task<IResult> GetUserServicesAsync(
        HttpRequest request,
        ITelegramInitDataValidator validator,
        INrnServicesStore store,
        CancellationToken cancellationToken)
    {
        if (!request.Headers.TryGetValue(TelegramApiHeaders.InitData, out var values) ||
            values.Count != 1)
        {
            return Results.Unauthorized();
        }

        var initData = values[0];
        if (string.IsNullOrWhiteSpace(initData) || initData.Length > MaxInitDataLength)
        {
            return Results.Unauthorized();
        }

        var validation = validator.Validate(initData);
        if (!validation.IsValid || validation.User is null)
        {
            return Results.Unauthorized();
        }

        var services = await store.GetUserServicesAsync(
            validation.User.Id,
            cancellationToken);
        return Results.Ok(services);
    }

    private static async Task<IResult> PurchaseAsync(
        string productId,
        HttpRequest request,
        ITelegramInitDataValidator validator,
        INrnServicesStore store,
        CancellationToken cancellationToken)
    {
        var user = ValidateUser(request, validator);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var purchase = await store.PurchaseAsync(user.Id, productId, cancellationToken);
        return purchase is null ? Results.NotFound() : Results.Ok(purchase);
    }

    private static TelegramUser? ValidateUser(
        HttpRequest request,
        ITelegramInitDataValidator validator)
    {
        if (!request.Headers.TryGetValue(TelegramApiHeaders.InitData, out var values) ||
            values.Count != 1)
        {
            return null;
        }

        var initData = values[0];
        if (string.IsNullOrWhiteSpace(initData) || initData.Length > MaxInitDataLength)
        {
            return null;
        }

        var validation = validator.Validate(initData);
        return validation.IsValid ? validation.User : null;
    }
}
