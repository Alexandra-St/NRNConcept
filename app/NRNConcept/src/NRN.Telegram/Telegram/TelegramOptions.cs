namespace NRN.Telegram.Telegram;

public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    public string? BotToken { get; init; }
    public int MaxAgeMinutes { get; init; } = 60;
    public string? SupportUrl { get; init; }
    // TODO(real-data): Replace null with the official Feedback destination when provided.
    public string? FeedbackUrl { get; init; }
}
