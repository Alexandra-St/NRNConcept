namespace NRN.Telegram.Telegram;

public interface ITelegramInitDataValidator
{
    TelegramValidationResult Validate(string? initData);
}
