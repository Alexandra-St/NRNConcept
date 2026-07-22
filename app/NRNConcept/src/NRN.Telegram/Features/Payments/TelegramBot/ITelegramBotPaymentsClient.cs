namespace NRN.Telegram.Features.Payments.TelegramBot;

public interface ITelegramBotPaymentsClient
{
    Task<string> CreateStarsInvoiceLinkAsync(
        string title,
        string description,
        string payload,
        int starsAmount,
        string priceLabel,
        CancellationToken cancellationToken = default);

    Task AnswerPreCheckoutAsync(
        string queryId,
        bool approved,
        string? errorMessage,
        CancellationToken cancellationToken = default);
}
