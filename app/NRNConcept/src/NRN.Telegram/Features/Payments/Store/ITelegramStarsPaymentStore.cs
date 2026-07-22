using NRN.Telegram.Features.Services.Contracts;

namespace NRN.Telegram.Features.Payments.Store;

public sealed record PendingStarsInvoice(
    string Payload,
    string ProductName,
    string ProductDescription,
    int StarsAmount);

public interface ITelegramStarsPaymentStore
{
    Task<PendingStarsInvoice?> CreatePendingAsync(
        long telegramUserId,
        string productId,
        int starsAmount,
        CancellationToken cancellationToken = default);

    Task<bool> CanCheckoutAsync(
        string payload,
        long telegramUserId,
        string currency,
        int totalAmount,
        CancellationToken cancellationToken = default);

    Task<ConnectedServiceDto?> CompleteAsync(
        string payload,
        long telegramUserId,
        string currency,
        int totalAmount,
        string telegramChargeId,
        CancellationToken cancellationToken = default);
}
