namespace NRN.Telegram.Telegram;

public enum TelegramValidationError
{
    None,
    MissingData,
    MissingBotToken,
    MalformedData,
    InvalidSignature,
    Expired,
    MissingUser
}

public sealed record TelegramValidationResult(
    bool IsValid,
    TelegramUser? User,
    string? QueryId,
    TelegramValidationError Error)
{
    public static TelegramValidationResult Success(TelegramUser user, string? queryId) =>
        new(true, user, queryId, TelegramValidationError.None);

    public static TelegramValidationResult Failure(TelegramValidationError error) =>
        new(false, null, null, error);
}
