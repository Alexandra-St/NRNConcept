using NRN.Telegram.Features.Services.Contracts;

namespace NRN.Telegram.Features.Services.Api;

public interface INrnServicesApi
{
    Task<UserServicesResponse> GetUserServicesAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default);

    Task<ServiceCatalogResponse> GetCatalogAsync(
        CancellationToken cancellationToken = default);

    Task<PurchaseServiceResponse?> PurchaseAsync(
        long telegramUserId,
        string productId,
        CancellationToken cancellationToken = default);
}
