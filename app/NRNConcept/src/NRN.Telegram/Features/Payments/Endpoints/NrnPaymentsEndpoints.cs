using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using NRN.Telegram.Features.Payments.Contracts;
using NRN.Telegram.Features.Payments.Store;
using NRN.Telegram.Features.Payments.TelegramBot;
using NRN.Telegram.Features.Services.Api;
using NRN.Telegram.Telegram;
using NRN.Telegram.Localization;

namespace NRN.Telegram.Features.Payments.Endpoints;

public static class NrnPaymentsEndpoints
{
    private const int MaxInitDataLength = 16 * 1024;
    private const string WebhookSecretHeader = "X-Telegram-Bot-Api-Secret-Token";

    public static void MapNrnPaymentsApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                "/api/v1/payments/stars/{productId}/invoice",
                CreateStarsInvoiceAsync)
            .WithName("CreateStarsInvoice");
        endpoints.MapPost("/api/telegram/webhook", HandleWebhookAsync)
            .WithName("HandleTelegramPaymentWebhook");
    }

    private static async Task<IResult> CreateStarsInvoiceAsync(
        string productId,
        HttpRequest request,
        ITelegramInitDataValidator validator,
        IOptions<TelegramPaymentsOptions> paymentOptions,
        ITelegramStarsPaymentStore store,
        ITelegramBotPaymentsClient telegram,
        MiniAppPreferencesState preferences,
        MiniAppLocalizer localizer,
        CancellationToken cancellationToken)
    {
        var user = ValidateUser(request, validator);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        if (!paymentOptions.Value.TryGetStarsPrice(productId, out var starsAmount))
        {
            return Results.Conflict(new { error = "Stars price is not configured." });
        }

        preferences.SetLanguage(request.Headers[TelegramApiHeaders.Language].FirstOrDefault());

        var order = await store.CreatePendingAsync(
            user.Id,
            productId,
            starsAmount,
            cancellationToken);
        if (order is null)
        {
            return Results.NotFound();
        }

        var invoiceUrl = await telegram.CreateStarsInvoiceLinkAsync(
            localizer.ProductName(productId, order.ProductName),
            localizer.ProductDescription(productId, order.ProductDescription),
            order.Payload,
            order.StarsAmount,
            localizer["catalog.connection"],
            cancellationToken);
        return Results.Ok(new StarsInvoiceResponse(invoiceUrl, order.StarsAmount));
    }

    private static async Task<IResult> HandleWebhookAsync(
        HttpRequest request,
        TelegramPaymentUpdate update,
        IOptions<TelegramPaymentsOptions> paymentOptions,
        ITelegramStarsPaymentStore store,
        ITelegramBotPaymentsClient telegram,
        MiniAppPreferencesState preferences,
        MiniAppLocalizer localizer,
        CancellationToken cancellationToken)
    {
        if (!HasValidWebhookSecret(request, paymentOptions.Value.WebhookSecret))
        {
            return Results.Unauthorized();
        }

        if (update.PreCheckoutQuery is { } checkout)
        {
            preferences.SetLanguage(checkout.From.LanguageCode);
            var approved = await store.CanCheckoutAsync(
                checkout.InvoicePayload,
                checkout.From.Id,
                checkout.Currency,
                checkout.TotalAmount,
                cancellationToken);
            await telegram.AnswerPreCheckoutAsync(
                checkout.Id,
                approved,
                approved ? null : localizer["payment.orderInvalid"],
                cancellationToken);
            return Results.Ok();
        }

        if (update.Message is { From: { } user, SuccessfulPayment: { } payment })
        {
            await store.CompleteAsync(
                payment.InvoicePayload,
                user.Id,
                payment.Currency,
                payment.TotalAmount,
                payment.TelegramPaymentChargeId,
                cancellationToken);
        }

        return Results.Ok();
    }

    private static bool HasValidWebhookSecret(HttpRequest request, string? expected)
    {
        if (string.IsNullOrWhiteSpace(expected) ||
            !request.Headers.TryGetValue(WebhookSecretHeader, out var suppliedValues) ||
            suppliedValues.Count != 1)
        {
            return false;
        }

        var supplied = suppliedValues[0] ?? string.Empty;
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var suppliedBytes = Encoding.UTF8.GetBytes(supplied);
        return expectedBytes.Length == suppliedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes);
    }

    private static TelegramUser? ValidateUser(
        HttpRequest request,
        ITelegramInitDataValidator validator)
    {
        if (!request.Headers.TryGetValue(TelegramApiHeaders.InitData, out var values) ||
            values.Count != 1)
        {
            return null;
        }

        var initData = values[0];
        if (string.IsNullOrWhiteSpace(initData) || initData.Length > MaxInitDataLength)
        {
            return null;
        }

        var validation = validator.Validate(initData);
        return validation.IsValid ? validation.User : null;
    }
}
