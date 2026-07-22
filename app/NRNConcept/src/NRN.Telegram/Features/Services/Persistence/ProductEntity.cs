namespace NRN.Telegram.Features.Services.Persistence;

using NRN.Telegram.Features.Services.Contracts;

public sealed class ProductEntity
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public int ConnectionFeeMinor { get; init; }
    public int MonthlyPriceMinor { get; init; }
    public required string Currency { get; init; }
    public ServicePlanKind Kind { get; init; }
    public string? CountryCode { get; init; }
    public required string FlagEmoji { get; init; }
    public bool IncludesPhoneNumber { get; init; }
    public required string Accent { get; init; }
    public bool IsPopular { get; init; }
    public bool ConnectionPriceIsFrom { get; init; }
    public bool MonthlyPriceIsFrom { get; init; }

    public ICollection<UserSubscriptionEntity> Subscriptions { get; init; } = [];
    public ICollection<PurchaseEntity> Purchases { get; init; } = [];
}
