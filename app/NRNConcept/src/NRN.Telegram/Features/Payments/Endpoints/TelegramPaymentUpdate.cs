using System.Text.Json.Serialization;

namespace NRN.Telegram.Features.Payments.Endpoints;

public sealed record TelegramPaymentUpdate(
    [property: JsonPropertyName("pre_checkout_query")] TelegramPreCheckoutQuery? PreCheckoutQuery,
    [property: JsonPropertyName("message")] TelegramPaymentMessage? Message);

public sealed record TelegramPreCheckoutQuery(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("from")] TelegramPaymentUser From,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("total_amount")] int TotalAmount,
    [property: JsonPropertyName("invoice_payload")] string InvoicePayload);

public sealed record TelegramPaymentMessage(
    [property: JsonPropertyName("from")] TelegramPaymentUser? From,
    [property: JsonPropertyName("successful_payment")] TelegramSuccessfulPayment? SuccessfulPayment);

public sealed record TelegramPaymentUser(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("language_code")] string? LanguageCode = null);

public sealed record TelegramSuccessfulPayment(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("total_amount")] int TotalAmount,
    [property: JsonPropertyName("invoice_payload")] string InvoicePayload,
    [property: JsonPropertyName("telegram_payment_charge_id")] string TelegramPaymentChargeId);
