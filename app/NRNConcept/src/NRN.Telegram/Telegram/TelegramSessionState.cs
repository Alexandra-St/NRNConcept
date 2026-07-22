namespace NRN.Telegram.Telegram;

public enum TelegramSessionStatus
{
    Loading,
    Preview,
    Authenticated,
    Rejected
}

public sealed class TelegramSessionState(ITelegramInitDataValidator validator)
{
    public TelegramSessionStatus Status { get; private set; } = TelegramSessionStatus.Loading;
    public TelegramUser? User { get; private set; }
    public string? QueryId { get; private set; }
    public TelegramValidationError? ValidationError { get; private set; }
    internal string? ValidatedInitData { get; private set; }

    public event Action? Changed;

    public void Initialize(string? initData)
    {
        if (string.IsNullOrWhiteSpace(initData))
        {
            UsePreviewMode();
            return;
        }

        var result = validator.Validate(initData);
        Status = result.IsValid
            ? TelegramSessionStatus.Authenticated
            : TelegramSessionStatus.Rejected;
        User = result.User;
        QueryId = result.QueryId;
        ValidationError = result.IsValid ? null : result.Error;
        ValidatedInitData = result.IsValid ? initData : null;
        Changed?.Invoke();
    }

    public void UsePreviewMode()
    {
        Status = TelegramSessionStatus.Preview;
        User = null;
        QueryId = null;
        ValidationError = null;
        ValidatedInitData = null;
        Changed?.Invoke();
    }
}
