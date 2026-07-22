using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace NRN.Telegram.Telegram;

public sealed class TelegramInitDataValidator(
    IOptions<TelegramOptions> options,
    TimeProvider timeProvider) : ITelegramInitDataValidator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TelegramValidationResult Validate(string? initData)
    {
        if (string.IsNullOrWhiteSpace(initData))
        {
            return TelegramValidationResult.Failure(TelegramValidationError.MissingData);
        }

        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.BotToken))
        {
            return TelegramValidationResult.Failure(TelegramValidationError.MissingBotToken);
        }

        Dictionary<string, string> fields;
        byte[] receivedHash;

        try
        {
            fields = QueryHelpers.ParseQuery(initData)
                .ToDictionary(pair => pair.Key, pair => pair.Value.ToString(), StringComparer.Ordinal);

            if (!fields.Remove("hash", out var hash) || hash.Length != 64)
            {
                return TelegramValidationResult.Failure(TelegramValidationError.MalformedData);
            }

            receivedHash = Convert.FromHexString(hash);
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException)
        {
            return TelegramValidationResult.Failure(TelegramValidationError.MalformedData);
        }

        var dataCheckString = string.Join('\n', fields
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => $"{pair.Key}={pair.Value}"));

        var secretKey = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("WebAppData"),
            Encoding.UTF8.GetBytes(settings.BotToken));
        var calculatedHash = HMACSHA256.HashData(
            secretKey,
            Encoding.UTF8.GetBytes(dataCheckString));

        if (!CryptographicOperations.FixedTimeEquals(calculatedHash, receivedHash))
        {
            return TelegramValidationResult.Failure(TelegramValidationError.InvalidSignature);
        }

        if (!TryValidateAuthDate(fields, settings.MaxAgeMinutes))
        {
            return TelegramValidationResult.Failure(TelegramValidationError.Expired);
        }

        if (!fields.TryGetValue("user", out var serializedUser))
        {
            return TelegramValidationResult.Failure(TelegramValidationError.MissingUser);
        }

        try
        {
            var user = JsonSerializer.Deserialize<TelegramUser>(serializedUser, JsonOptions);
            return user is { Id: > 0, FirstName.Length: > 0 }
                ? TelegramValidationResult.Success(user, fields.GetValueOrDefault("query_id"))
                : TelegramValidationResult.Failure(TelegramValidationError.MissingUser);
        }
        catch (JsonException)
        {
            return TelegramValidationResult.Failure(TelegramValidationError.MalformedData);
        }
    }

    private bool TryValidateAuthDate(IReadOnlyDictionary<string, string> fields, int maxAgeMinutes)
    {
        if (!fields.TryGetValue("auth_date", out var rawAuthDate) ||
            !long.TryParse(rawAuthDate, NumberStyles.None, CultureInfo.InvariantCulture, out var unixSeconds))
        {
            return false;
        }

        DateTimeOffset authDate;
        try
        {
            authDate = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }

        var now = timeProvider.GetUtcNow();
        var maxAge = TimeSpan.FromMinutes(Math.Max(1, maxAgeMinutes));
        return authDate <= now.AddMinutes(5) && now - authDate <= maxAge;
    }
}
