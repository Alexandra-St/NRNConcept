namespace NRN.Telegram.Features.Payments.Contracts;

public sealed record StarsInvoiceResponse(
    string InvoiceUrl,
    int StarsAmount);
