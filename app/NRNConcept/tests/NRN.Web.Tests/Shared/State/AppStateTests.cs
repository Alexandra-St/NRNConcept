using NRN.Web.Shared.State;

namespace NRN.Web.Tests.Shared.State;

public sealed class AppStateTests
{
    [Fact]
    public void SetLocale_AcceptsSupportedLocaleAndRaisesEvent()
    {
        var state = new AppState();
        var eventCount = 0;
        state.LocaleChanged += () => eventCount++;

        state.SetLocale("EN");

        Assert.Equal("en", state.Locale);
        Assert.Equal(1, eventCount);
    }

    [Fact]
    public void SetLocale_IgnoresUnsupportedLocale()
    {
        var state = new AppState();

        state.SetLocale("de");

        Assert.Equal("ru", state.Locale);
    }

    [Fact]
    public void SetTheme_AcceptsSupportedThemeAndRaisesEvent()
    {
        var state = new AppState();
        var eventCount = 0;
        state.ThemeChanged += () => eventCount++;

        state.SetTheme("DARK");

        Assert.Equal("dark", state.Theme);
        Assert.Equal(1, eventCount);
    }

    [Fact]
    public void SetTheme_IgnoresUnsupportedTheme()
    {
        var state = new AppState();

        state.SetTheme("neon");

        Assert.Equal("light", state.Theme);
    }
}
