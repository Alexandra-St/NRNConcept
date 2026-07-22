namespace NRN.Telegram.Features.Services.Persistence;

public sealed class UserAccountEntity
{
    public long TelegramUserId { get; init; }
    public int BalanceMinor { get; set; }
    public required string Currency { get; init; }
}
