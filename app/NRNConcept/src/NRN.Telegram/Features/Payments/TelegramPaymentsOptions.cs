namespace NRN.Telegram.Features.Payments;

public sealed class TelegramPaymentsOptions
{
    public const string SectionName = "TelegramPayments";

    public string? WebhookSecret { get; init; }
    public Dictionary<string, int> StarsPrices { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public bool TryGetStarsPrice(string productId, out int price) =>
        StarsPrices.TryGetValue(productId, out price) && price > 0;
}
