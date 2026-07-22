using NRN.Telegram.Features.Payments.Contracts;

namespace NRN.Telegram.Features.Payments.Api;

public interface INrnPaymentsApi
{
    Task<StarsInvoiceResponse?> CreateStarsInvoiceAsync(
        long telegramUserId,
        string productId,
        CancellationToken cancellationToken = default);
}
