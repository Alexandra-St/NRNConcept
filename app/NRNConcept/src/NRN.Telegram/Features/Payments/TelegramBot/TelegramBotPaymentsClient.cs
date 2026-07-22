using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Features.Payments.TelegramBot;

public sealed class TelegramBotPaymentsClient(
    HttpClient httpClient,
    IOptions<TelegramOptions> telegramOptions) : ITelegramBotPaymentsClient
{
    public async Task<string> CreateStarsInvoiceLinkAsync(
        string title,
        string description,
        string payload,
        int starsAmount,
        string priceLabel,
        CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<string>(
            "createInvoiceLink",
            new
            {
                title = title.Length <= 32 ? title : title[..32],
                description = description.Length <= 255 ? description : description[..255],
                payload,
                currency = "XTR",
                prices = new[] { new { label = priceLabel, amount = starsAmount } }
            },
            cancellationToken);
        return response;
    }

    public async Task AnswerPreCheckoutAsync(
        string queryId,
        bool approved,
        string? errorMessage,
        CancellationToken cancellationToken = default) =>
        await PostAsync<bool>(
            "answerPreCheckoutQuery",
            new
            {
                pre_checkout_query_id = queryId,
                ok = approved,
                error_message = approved ? null : errorMessage ?? "The payment could not be processed."
            },
            cancellationToken);

    private async Task<T> PostAsync<T>(
        string method,
        object payload,
        CancellationToken cancellationToken)
    {
        var token = telegramOptions.Value.BotToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Telegram bot token is not configured.");
        }

        using var response = await httpClient.PostAsJsonAsync(
            $"https://api.telegram.org/bot{token}/{method}",
            payload,
            cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<BotApiResponse<T>>(
            cancellationToken: cancellationToken);
        if (!response.IsSuccessStatusCode || result is null || !result.Ok || result.Result is null)
        {
            throw new InvalidOperationException(
                result?.Description ?? $"Telegram Bot API returned HTTP {(int)response.StatusCode}.");
        }

        return result.Result;
    }

    private sealed record BotApiResponse<T>(
        [property: JsonPropertyName("ok")] bool Ok,
        [property: JsonPropertyName("result")] T? Result,
        [property: JsonPropertyName("description")] string? Description);
}
