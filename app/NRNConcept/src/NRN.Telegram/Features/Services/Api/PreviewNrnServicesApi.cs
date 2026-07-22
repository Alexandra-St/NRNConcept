using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Features.Services.Store;

namespace NRN.Telegram.Features.Services.Api;

public sealed class PreviewNrnServicesApi(INrnServicesStore store) : IPreviewNrnServicesApi
{
    public Task<UserServicesResponse> GetUserServicesAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default) =>
        store.GetUserServicesAsync(telegramUserId, cancellationToken);

    public Task<ServiceCatalogResponse> GetCatalogAsync(
        CancellationToken cancellationToken = default) =>
        store.GetCatalogAsync(cancellationToken);

    public Task<PurchaseServiceResponse?> PurchaseAsync(
        long telegramUserId,
        string productId,
        CancellationToken cancellationToken = default) =>
        store.PurchaseAsync(telegramUserId, productId, cancellationToken);
}
