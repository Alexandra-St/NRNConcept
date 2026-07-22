using NRN.Telegram.Features.Services.Api;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Telegram;
using System.Net;

namespace NRN.Telegram.Features.Services.State;

public enum UserServicesStatus
{
    NotStarted,
    Loading,
    Ready,
    Unauthorized,
    Error
}

public sealed class UserServicesState(
    INrnServicesApi api,
    IPreviewNrnServicesApi previewApi)
{
    private const long PreviewUserId = 0;

    public UserServicesStatus Status { get; private set; } = UserServicesStatus.NotStarted;
    public bool IsPreview { get; private set; }
    public long? TelegramUserId { get; private set; }
    public IReadOnlyList<ConnectedServiceDto> ConnectedServices { get; private set; } = [];
    public IReadOnlyList<ProductOfferDto> ProductOffers { get; private set; } = [];
    public decimal Balance { get; private set; }
    public string Currency { get; private set; } = "EUR";
    public bool IsPurchasing { get; private set; }
    public bool IsRefreshing { get; private set; }
    public bool HasRefreshError { get; private set; }
    public string? LoadErrorKey { get; private set; }
    public PurchaseServiceResponse? LastPurchase { get; private set; }
    public string? PurchaseError { get; private set; }

    public event Action? Changed;

    public ProductOfferDto? FindProduct(string productId) =>
        ProductOffers.FirstOrDefault(item =>
            string.Equals(item.Id, productId, StringComparison.OrdinalIgnoreCase));

    public async Task LoadAsync(
        TelegramSessionState session,
        CancellationToken cancellationToken = default)
    {
        if (Status == UserServicesStatus.Loading || IsRefreshing)
        {
            return;
        }

        if (session.Status == TelegramSessionStatus.Rejected)
        {
            Reset(UserServicesStatus.Unauthorized);
            return;
        }

        var telegramUserId = session.Status switch
        {
            TelegramSessionStatus.Authenticated when session.User is not null => session.User.Id,
            TelegramSessionStatus.Preview => PreviewUserId,
            _ => (long?)null
        };

        if (telegramUserId is null)
        {
            Reset(UserServicesStatus.Unauthorized);
            return;
        }

        var preserveContent = Status == UserServicesStatus.Ready;
        Status = preserveContent ? UserServicesStatus.Ready : UserServicesStatus.Loading;
        IsRefreshing = preserveContent;
        HasRefreshError = false;
        LoadErrorKey = null;
        IsPreview = session.Status == TelegramSessionStatus.Preview;
        TelegramUserId = IsPreview ? null : telegramUserId;
        Changed?.Invoke();

        try
        {
            var selectedApi = IsPreview ? previewApi : api;
            var servicesTask = selectedApi.GetUserServicesAsync(telegramUserId.Value, cancellationToken);
            var catalogTask = selectedApi.GetCatalogAsync(cancellationToken);
            await Task.WhenAll(servicesTask, catalogTask);

            var services = await servicesTask;
            var catalog = await catalogTask;

            if (services.TelegramUserId != telegramUserId.Value)
            {
                HandleLoadFailure(preserveContent, "data.errorUnknown");
                return;
            }

            ConnectedServices = services.Services;
            Balance = services.Balance;
            Currency = services.Currency;
            ProductOffers = catalog.Offers;
            Status = UserServicesStatus.Ready;
            IsRefreshing = false;
            Changed?.Invoke();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            IsRefreshing = false;
            if (!preserveContent)
            {
                Status = UserServicesStatus.NotStarted;
            }
            Changed?.Invoke();
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            HandleLoadFailure(preserveContent, "data.errorUnauthorized");
        }
        catch (TaskCanceledException)
        {
            HandleLoadFailure(preserveContent, "data.errorTimeout");
        }
        catch (HttpRequestException exception)
        {
            HandleLoadFailure(preserveContent, LoadErrorFor(exception));
        }
        catch
        {
            HandleLoadFailure(preserveContent, "data.errorUnknown");
        }
    }

    public async Task<PurchaseServiceResponse?> PurchaseAsync(
        string productId,
        TelegramSessionState session,
        CancellationToken cancellationToken = default)
    {
        if (IsPurchasing)
        {
            return null;
        }

        var telegramUserId = session.Status switch
        {
            TelegramSessionStatus.Authenticated when session.User is not null => session.User.Id,
            TelegramSessionStatus.Preview => PreviewUserId,
            _ => (long?)null
        };
        if (telegramUserId is null)
        {
            PurchaseError = "purchase.telegramRequired";
            Changed?.Invoke();
            return null;
        }

        IsPurchasing = true;
        PurchaseError = null;
        Changed?.Invoke();

        try
        {
            var selectedApi = session.Status == TelegramSessionStatus.Preview ? previewApi : api;
            var purchase = await selectedApi.PurchaseAsync(
                telegramUserId.Value,
                productId,
                cancellationToken);
            if (purchase is null || purchase.TelegramUserId != telegramUserId.Value)
            {
                PurchaseError = "purchase.unavailable";
                return null;
            }

            LastPurchase = purchase;
            Balance = purchase.Balance;
            Currency = purchase.Currency;
            if (purchase.Service is not null)
            {
                var services = ConnectedServices
                    .Where(item => !string.Equals(item.Id, purchase.Service.Id, StringComparison.OrdinalIgnoreCase))
                    .Append(purchase.Service)
                    .OrderBy(item => item.Name)
                    .ToArray();
                ConnectedServices = services;
            }
            return purchase;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            PurchaseError = "purchase.telegramRequired";
            return null;
        }
        catch (TaskCanceledException)
        {
            PurchaseError = "purchase.timeout";
            return null;
        }
        catch (HttpRequestException exception)
        {
            PurchaseError = PurchaseErrorFor(exception);
            return null;
        }
        catch
        {
            PurchaseError = "purchase.failed";
            return null;
        }
        finally
        {
            IsPurchasing = false;
            Changed?.Invoke();
        }
    }

    private void Reset(UserServicesStatus status, string? loadErrorKey = null)
    {
        Status = status;
        IsPreview = false;
        TelegramUserId = null;
        ConnectedServices = [];
        ProductOffers = [];
        Balance = 0;
        Currency = "EUR";
        IsPurchasing = false;
        IsRefreshing = false;
        HasRefreshError = false;
        LoadErrorKey = loadErrorKey;
        LastPurchase = null;
        PurchaseError = null;
        Changed?.Invoke();
    }

    private void HandleLoadFailure(bool preserveContent, string errorKey)
    {
        if (!preserveContent)
        {
            Reset(UserServicesStatus.Error, errorKey);
            return;
        }

        LoadErrorKey = errorKey;
        IsRefreshing = false;
        HasRefreshError = true;
        Changed?.Invoke();
    }

    private static string LoadErrorFor(HttpRequestException exception) => exception.StatusCode switch
    {
        HttpStatusCode.Unauthorized => "data.errorUnauthorized",
        HttpStatusCode.RequestTimeout or HttpStatusCode.GatewayTimeout => "data.errorTimeout",
        >= HttpStatusCode.InternalServerError => "data.errorServer",
        null => "data.errorOffline",
        _ => "data.errorUnknown"
    };

    private static string PurchaseErrorFor(HttpRequestException exception) => exception.StatusCode switch
    {
        HttpStatusCode.Unauthorized => "purchase.telegramRequired",
        HttpStatusCode.Conflict => "purchase.conflict",
        HttpStatusCode.RequestTimeout or HttpStatusCode.GatewayTimeout => "purchase.timeout",
        null => "purchase.offline",
        _ => "purchase.failed"
    };
}
