using NRN.Telegram.Features.Services.Contracts;

namespace NRN.Telegram.Features.Services.Store;

public interface INrnServicesStore
{
    Task<UserServicesResponse> GetUserServicesAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default);

    Task<ServiceCatalogResponse> GetCatalogAsync(
        CancellationToken cancellationToken = default);

    Task<ProductOfferDto?> GetProductAsync(
        string productId,
        CancellationToken cancellationToken = default);

    Task<PurchaseServiceResponse?> PurchaseAsync(
        long telegramUserId,
        string productId,
        CancellationToken cancellationToken = default);
}
