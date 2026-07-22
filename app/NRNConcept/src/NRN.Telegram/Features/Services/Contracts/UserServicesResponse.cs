namespace NRN.Telegram.Features.Services.Contracts;

public sealed record UserServicesResponse(
    long TelegramUserId,
    decimal Balance,
    string Currency,
    IReadOnlyList<ConnectedServiceDto> Services);

public sealed record ServiceCatalogResponse(
    IReadOnlyList<ProductOfferDto> Offers);

public enum PurchaseServiceStatus
{
    Completed,
    AlreadyConnected,
    InsufficientBalance
}

public sealed record PurchaseServiceResponse(
    long TelegramUserId,
    PurchaseServiceStatus Status,
    ConnectedServiceDto? Service,
    decimal ChargedAmount,
    decimal Balance,
    string Currency);
