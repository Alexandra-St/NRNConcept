namespace NRN.Telegram.Features.Services.Persistence;

public sealed class PurchaseEntity
{
    public Guid Id { get; init; }
    public long TelegramUserId { get; init; }
    public required string ProductId { get; init; }
    public int AmountMinor { get; init; }
    public required string Currency { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }

    public ProductEntity Product { get; init; } = null!;
}
