namespace NRN.Web.Shared.State;

public class AppState
{
    private static readonly HashSet<string> SupportedLocales =
        new(StringComparer.OrdinalIgnoreCase) { "en", "ru" };

    public string Locale { get; private set; } = "ru";
    public string Theme { get; private set; } = "light";

    public event Action? ThemeChanged;
    public event Action? LocaleChanged;

    public void SetLocale(string locale)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locale);

        var normalized = locale.Trim().ToLowerInvariant();
        if (!SupportedLocales.Contains(normalized) ||
            string.Equals(Locale, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Locale = normalized;
        LocaleChanged?.Invoke();
    }

    public void SetTheme(string theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme);

        var normalized = theme.Trim().ToLowerInvariant();
        if (normalized is not ("light" or "dark") ||
            string.Equals(Theme, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Theme = normalized;
        ThemeChanged?.Invoke();
    }
}
