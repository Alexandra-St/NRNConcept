namespace NRN.Telegram.Features.Help;

public enum SupportLinkKind
{
    Telegram,
    External
}

public sealed record SupportLink(Uri Url, SupportLinkKind Kind)
{
    private static readonly HashSet<string> TelegramHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "t.me",
        "www.t.me",
        "telegram.me",
        "www.telegram.me"
    };

    public static SupportLink? Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(uri.Host) ||
            !string.IsNullOrEmpty(uri.UserInfo))
        {
            return null;
        }

        var kind = TelegramHosts.Contains(uri.IdnHost)
            ? SupportLinkKind.Telegram
            : SupportLinkKind.External;
        return new SupportLink(uri, kind);
    }

    public static bool IsValidOrEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) || Create(value) is not null;
}
