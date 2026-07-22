using NRN.Telegram.Features.Services.Contracts;

namespace NRN.Telegram.Features.Services.Persistence;

public sealed class UserSubscriptionEntity
{
    public long TelegramUserId { get; init; }
    public required string ProductId { get; init; }
    public ConnectedServiceStatus Status { get; init; }
    public required string StatusDetail { get; init; }

    public ProductEntity Product { get; init; } = null!;
}
