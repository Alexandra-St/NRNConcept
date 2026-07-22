using System.Net;
using Microsoft.AspNetCore.Components;
using NRN.Telegram.Features.Payments.Contracts;
using NRN.Telegram.Features.Services.Api;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Features.Payments.Api;

public sealed class HttpNrnPaymentsApi(
    HttpClient httpClient,
    NavigationManager navigation,
    TelegramSessionState session,
    MiniAppPreferencesState preferences) : INrnPaymentsApi
{
    public async Task<StarsInvoiceResponse?> CreateStarsInvoiceAsync(
        long telegramUserId,
        string productId,
        CancellationToken cancellationToken = default)
    {
        if (session.Status != TelegramSessionStatus.Authenticated ||
            session.User?.Id != telegramUserId ||
            string.IsNullOrWhiteSpace(session.ValidatedInitData))
        {
            throw new UnauthorizedAccessException("A validated Telegram session is required.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(new Uri(navigation.BaseUri), $"api/v1/payments/stars/{Uri.EscapeDataString(productId)}/invoice"));
        request.Headers.TryAddWithoutValidation(
            TelegramApiHeaders.InitData,
            session.ValidatedInitData);
        request.Headers.TryAddWithoutValidation(
            TelegramApiHeaders.Language,
            preferences.Language);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Conflict)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StarsInvoiceResponse>(cancellationToken);
    }
}
