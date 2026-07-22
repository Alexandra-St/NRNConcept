namespace NRN.Telegram.Telegram;

public sealed class MiniAppPreferencesState
{
    public string Language { get; private set; } = "ru";
    public event Action? Changed;

    public void Initialize(string? savedLanguage, string? telegramLanguage)
    {
        if (string.IsNullOrWhiteSpace(savedLanguage) && string.IsNullOrWhiteSpace(telegramLanguage))
        {
            return;
        }

        var preferred = string.IsNullOrWhiteSpace(savedLanguage) ? telegramLanguage : savedLanguage;
        SetLanguage(preferred);
    }

    public void SetLanguage(string? language)
    {
        var normalized = language?.StartsWith("en", StringComparison.OrdinalIgnoreCase) == true ? "en" : "ru";
        if (Language == normalized)
        {
            return;
        }

        Language = normalized;
        Changed?.Invoke();
    }
}