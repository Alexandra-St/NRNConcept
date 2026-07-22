using NRN.Telegram.Localization;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Tests.Localization;

public sealed class MiniAppLocalizerTests
{
    [Theory]
    [InlineData("ru")]
    [InlineData("en")]
    public void EverySupportedLanguageContainsEveryKey(string language)
    {
        Assert.Empty(MiniAppLocalizer.MissingKeys(language));
    }

    [Fact]
    public void TelegramLanguageSelectsEnglishAndRussianFallback()
    {
        var preferences = new MiniAppPreferencesState();

        preferences.Initialize(null, "en-US");
        Assert.Equal("en", preferences.Language);

        preferences.Initialize(null, "et");
        Assert.Equal("ru", preferences.Language);
    }

    [Fact]
    public void SavedLanguageOverridesTelegramLanguage()
    {
        var preferences = new MiniAppPreferencesState();

        preferences.Initialize("ru", "en");

        Assert.Equal("ru", preferences.Language);
    }

    [Fact]
    public void MissingClientLanguageDoesNotOverwriteServerPreference()
    {
        var preferences = new MiniAppPreferencesState();
        preferences.SetLanguage("en");

        preferences.Initialize(null, null);

        Assert.Equal("en", preferences.Language);
    }

    [Fact]
    public void ProductAndStatusDetailsAreLocalized()
    {
        var preferences = new MiniAppPreferencesState();
        preferences.SetLanguage("en");
        var localizer = new MiniAppLocalizer(preferences);

        Assert.Equal("Data and voice eSIM with instant digital delivery and no physical card.",
            localizer.ProductDescription("esim", "fallback"));
        Assert.Equal("balance 0 EUR", localizer.ServiceStatusDetail("баланс 0 EUR"));
        Assert.Equal("paid with 25 Stars", localizer.ServiceStatusDetail("оплачено 25 Stars"));
    }
}
