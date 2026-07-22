using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Tests.Telegram;

public sealed class TelegramInitDataValidatorTests
{
    private const string BotToken = "123456:test-token";
    private static readonly DateTimeOffset Now = new(2026, 7, 14, 18, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Validate_ReturnsUser_WhenSignatureAndTimestampAreValid()
    {
        var validator = CreateValidator();
        var initData = CreateInitData(Now);

        var result = validator.Validate(initData);

        Assert.True(result.IsValid);
        Assert.Equal(TelegramValidationError.None, result.Error);
        Assert.Equal(42, result.User?.Id);
        Assert.Equal("Alex", result.User?.FirstName);
        Assert.Equal("query-1", result.QueryId);
    }

    [Fact]
    public void Validate_RejectsData_WhenSignedValueWasChanged()
    {
        var validator = CreateValidator();
        var initData = CreateInitData(Now).Replace("Alex", "Mallory", StringComparison.Ordinal);

        var result = validator.Validate(initData);

        Assert.False(result.IsValid);
        Assert.Equal(TelegramValidationError.InvalidSignature, result.Error);
    }

    [Fact]
    public void Validate_RejectsData_WhenAuthDateIsExpired()
    {
        var validator = CreateValidator();
        var initData = CreateInitData(Now.AddHours(-2));

        var result = validator.Validate(initData);

        Assert.False(result.IsValid);
        Assert.Equal(TelegramValidationError.Expired, result.Error);
    }

    [Fact]
    public void Validate_RejectsData_WhenBotTokenIsMissing()
    {
        var options = Options.Create(new TelegramOptions());
        var validator = new TelegramInitDataValidator(options, new FixedTimeProvider(Now));

        var result = validator.Validate(CreateInitData(Now));

        Assert.False(result.IsValid);
        Assert.Equal(TelegramValidationError.MissingBotToken, result.Error);
    }

    private static TelegramInitDataValidator CreateValidator()
    {
        var options = Options.Create(new TelegramOptions
        {
            BotToken = BotToken,
            MaxAgeMinutes = 60
        });
        return new TelegramInitDataValidator(options, new FixedTimeProvider(Now));
    }

    private static string CreateInitData(DateTimeOffset authDate)
    {
        var fields = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["auth_date"] = authDate.ToUnixTimeSeconds().ToString(),
            ["query_id"] = "query-1",
            ["user"] = "{\"id\":42,\"first_name\":\"Alex\",\"last_name\":\"N\",\"username\":\"alex\",\"language_code\":\"ru\"}"
        };
        var dataCheckString = string.Join('\n', fields.Select(pair => $"{pair.Key}={pair.Value}"));
        var secretKey = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("WebAppData"),
            Encoding.UTF8.GetBytes(BotToken));
        var hash = Convert.ToHexString(HMACSHA256.HashData(
            secretKey,
            Encoding.UTF8.GetBytes(dataCheckString))).ToLowerInvariant();

        return string.Join('&', fields.Select(pair =>
            $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}")) + $"&hash={hash}";
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
