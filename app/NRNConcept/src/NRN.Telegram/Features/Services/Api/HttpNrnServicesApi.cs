using Microsoft.AspNetCore.Components;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Features.Services.Api;

public sealed class HttpNrnServicesApi(
    HttpClient httpClient,
    NavigationManager navigation,
    TelegramSessionState session) : INrnServicesApi
{
    // TODO(real-data): Point this boundary at the existing Narayana bot/backend API.
    // The current endpoints are local MVP contracts backed by SQLite.
    public async Task<UserServicesResponse> GetUserServicesAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default)
    {
        if (session.Status != TelegramSessionStatus.Authenticated ||
            session.User?.Id != telegramUserId ||
            string.IsNullOrWhiteSpace(session.ValidatedInitData))
        {
            throw new UnauthorizedAccessException("A validated Telegram session is required.");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            BuildUri("api/v1/services"));
        request.Headers.TryAddWithoutValidation(
            TelegramApiHeaders.InitData,
            session.ValidatedInitData);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserServicesResponse>(cancellationToken)
            ?? throw new InvalidOperationException("The services API returned an empty response.");
    }

    public async Task<ServiceCatalogResponse> GetCatalogAsync(
        CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<ServiceCatalogResponse>(
            BuildUri("api/v1/catalog"),
            cancellationToken)
        ?? throw new InvalidOperationException("The catalog API returned an empty response.");

    public async Task<ProductOfferDto?> GetProductAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            BuildUri($"api/v1/catalog/{Uri.EscapeDataString(productId)}"),
            cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductOfferDto>(cancellationToken);
    }

    public async Task<PurchaseServiceResponse?> PurchaseAsync(
        long telegramUserId,
        string productId,
        CancellationToken cancellationToken = default)
    {
        EnsureAuthenticatedUser(telegramUserId);
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            BuildUri($"api/v1/services/{Uri.EscapeDataString(productId)}"));
        request.Headers.TryAddWithoutValidation(
            TelegramApiHeaders.InitData,
            session.ValidatedInitData);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PurchaseServiceResponse>(cancellationToken);
    }

    private void EnsureAuthenticatedUser(long telegramUserId)
    {
        if (session.Status != TelegramSessionStatus.Authenticated ||
            session.User?.Id != telegramUserId ||
            string.IsNullOrWhiteSpace(session.ValidatedInitData))
        {
            throw new UnauthorizedAccessException("A validated Telegram session is required.");
        }
    }

    private Uri BuildUri(string relativePath) =>
        new(new Uri(navigation.BaseUri, UriKind.Absolute), relativePath);
}
