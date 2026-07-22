namespace NRN.Telegram.Features.Payments.Persistence;

public enum StarsPaymentStatus
{
    Pending,
    Paid
}

public sealed class StarsPaymentEntity
{
    public Guid Id { get; set; }
    public required string InvoicePayload { get; init; }
    public long TelegramUserId { get; set; }
    public required string ProductId { get; init; }
    public int StarsAmount { get; set; }
    public StarsPaymentStatus Status { get; set; }
    public string? TelegramChargeId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
}
