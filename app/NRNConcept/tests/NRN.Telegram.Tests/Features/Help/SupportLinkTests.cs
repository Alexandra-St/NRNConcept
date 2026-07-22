using NRN.Telegram.Features.Help;

namespace NRN.Telegram.Tests.Features.Help;

public sealed class SupportLinkTests
{
    [Theory]
    [InlineData("https://t.me/narayana_support")]
    [InlineData("https://telegram.me/narayana_support")]
    public void TelegramHostsUseTelegramNavigation(string value)
    {
        var link = SupportLink.Create(value);

        Assert.NotNull(link);
        Assert.Equal(SupportLinkKind.Telegram, link.Kind);
    }

    [Fact]
    public void RegularHttpsUrlUsesExternalNavigation()
    {
        var link = SupportLink.Create("https://narayana.im/feedback");

        Assert.NotNull(link);
        Assert.Equal(SupportLinkKind.External, link.Kind);
    }

    [Theory]
    [InlineData("http://t.me/narayana_support")]
    [InlineData("https://t.me@malicious.example/support")]
    [InlineData("/relative/support")]
    [InlineData("javascript:alert(1)")]
    public void UnsafeOrNonAbsoluteUrlIsRejected(string value)
    {
        Assert.Null(SupportLink.Create(value));
        Assert.False(SupportLink.IsValidOrEmpty(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MissingProductionUrlIsAllowed(string? value)
    {
        Assert.True(SupportLink.IsValidOrEmpty(value));
    }
}
