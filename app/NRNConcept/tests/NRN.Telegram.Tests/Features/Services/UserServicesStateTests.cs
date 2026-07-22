using NRN.Telegram.Features.Services.Api;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Features.Services.State;
using NRN.Telegram.Telegram;
using System.Net;

namespace NRN.Telegram.Tests.Features.Services;

public sealed class UserServicesStateTests
{
    [Fact]
    public async Task LoadAsync_UsesValidatedTelegramUserId()
    {
        var api = new RecordingServicesApi();
        var state = new UserServicesState(api, api);
        var session = AuthenticatedSession(4821);

        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        Assert.Equal(4821, api.RequestedTelegramUserId);
        Assert.Equal(4821, state.TelegramUserId);
        Assert.Equal(UserServicesStatus.Ready, state.Status);
        Assert.False(state.IsPreview);
        Assert.Equal(12m, state.Balance);
    }

    [Fact]
    public async Task LoadAsync_DoesNotCallApi_WhenTelegramSessionIsRejected()
    {
        var api = new RecordingServicesApi();
        var state = new UserServicesState(api, api);
        var session = new TelegramSessionState(new StubValidator(
            TelegramValidationResult.Failure(TelegramValidationError.InvalidSignature)));
        session.Initialize("invalid");

        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        Assert.Null(api.RequestedTelegramUserId);
        Assert.Equal(UserServicesStatus.Unauthorized, state.Status);
    }

    [Fact]
    public async Task LoadAsync_RejectsResponseForAnotherTelegramUser()
    {
        var api = new RecordingServicesApi(responseUserId: 999);
        var state = new UserServicesState(api, api);
        var session = AuthenticatedSession(4821);

        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        Assert.Equal(UserServicesStatus.Error, state.Status);
        Assert.Empty(state.ConnectedServices);
    }

    [Fact]
    public async Task LoadAsync_UsesSeparatePreviewContext_OutsideTelegram()
    {
        var api = new RecordingServicesApi();
        var state = new UserServicesState(api, api);
        var session = new TelegramSessionState(new StubValidator(
            TelegramValidationResult.Failure(TelegramValidationError.MissingData)));
        session.UsePreviewMode();

        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        Assert.Equal(0, api.RequestedTelegramUserId);
        Assert.Null(state.TelegramUserId);
        Assert.True(state.IsPreview);
        Assert.Equal(UserServicesStatus.Ready, state.Status);
    }

    [Fact]
    public async Task PurchaseAsync_UpdatesBalanceAndConnectedServices()
    {
        var api = new RecordingServicesApi();
        var state = new UserServicesState(api, api);
        var session = AuthenticatedSession(4821);
        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        var result = await state.PurchaseAsync(
            "esim",
            session,
            TestContext.Current.CancellationToken);

        Assert.Equal(PurchaseServiceStatus.Completed, result?.Status);
        Assert.Equal(3m, state.Balance);
        Assert.Contains(state.ConnectedServices, item => item.Id == "esim");
    }

    [Fact]
    public async Task PurchaseAsync_IgnoresASecondRequestWhileTheFirstIsRunning()
    {
        var api = new RecordingServicesApi
        {
            PendingPurchase = new TaskCompletionSource<PurchaseServiceResponse?>(
                TaskCreationOptions.RunContinuationsAsynchronously)
        };
        var state = new UserServicesState(api, api);
        var session = AuthenticatedSession(4821);
        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        var firstPurchase = state.PurchaseAsync("esim", session, TestContext.Current.CancellationToken);
        var duplicatePurchase = await state.PurchaseAsync(
            "esim",
            session,
            TestContext.Current.CancellationToken);

        Assert.Null(duplicatePurchase);
        Assert.Equal(1, api.PurchaseCalls);
        api.PendingPurchase.SetResult(api.CompletedPurchase(4821, "esim"));
        Assert.NotNull(await firstPurchase);
    }

    [Fact]
    public async Task LoadAsync_PreservesContentWhenBackgroundRefreshFails()
    {
        var api = new RecordingServicesApi();
        var state = new UserServicesState(api, api);
        var session = AuthenticatedSession(4821);
        await state.LoadAsync(session, TestContext.Current.CancellationToken);
        api.FailLoads = true;

        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        Assert.Equal(UserServicesStatus.Ready, state.Status);
        Assert.NotEmpty(state.ConnectedServices);
        Assert.True(state.HasRefreshError);
        Assert.False(state.IsRefreshing);
    }

    [Fact]
    public async Task LoadAsync_MapsUnauthorizedResponseToSafeUserMessage()
    {
        var api = new RecordingServicesApi
        {
            LoadFailure = new HttpRequestException(
                "technical detail",
                null,
                HttpStatusCode.Unauthorized)
        };
        var state = new UserServicesState(api, api);

        await state.LoadAsync(
            AuthenticatedSession(4821),
            TestContext.Current.CancellationToken);

        Assert.Equal(UserServicesStatus.Error, state.Status);
        Assert.Equal("data.errorUnauthorized", state.LoadErrorKey);
        Assert.DoesNotContain("technical", state.LoadErrorKey);
    }

    [Fact]
    public async Task LoadAsync_MapsTimeoutToSafeUserMessage()
    {
        var api = new RecordingServicesApi { LoadFailure = new TaskCanceledException() };
        var state = new UserServicesState(api, api);

        await state.LoadAsync(
            AuthenticatedSession(4821),
            TestContext.Current.CancellationToken);

        Assert.Equal(UserServicesStatus.Error, state.Status);
        Assert.Equal("data.errorTimeout", state.LoadErrorKey);
    }

    [Fact]
    public async Task PurchaseAsync_MapsConflictWithoutExposingHttpDetails()
    {
        var api = new RecordingServicesApi
        {
            PurchaseFailure = new HttpRequestException(
                "technical conflict",
                null,
                HttpStatusCode.Conflict)
        };
        var state = new UserServicesState(api, api);
        var session = AuthenticatedSession(4821);
        await state.LoadAsync(session, TestContext.Current.CancellationToken);

        var result = await state.PurchaseAsync(
            "esim",
            session,
            TestContext.Current.CancellationToken);

        Assert.Null(result);
        Assert.Equal("purchase.conflict", state.PurchaseError);
    }

    private static TelegramSessionState AuthenticatedSession(long userId)
    {
        var result = TelegramValidationResult.Success(
            new TelegramUser(userId, "Alex", null, null, "ru", null),
            "query");
        var session = new TelegramSessionState(new StubValidator(result));
        session.Initialize("signed");
        return session;
    }

    private sealed class StubValidator(TelegramValidationResult result) : ITelegramInitDataValidator
    {
        public TelegramValidationResult Validate(string? initData) => result;
    }

    private sealed class RecordingServicesApi(long? responseUserId = null) : IPreviewNrnServicesApi
    {
        public long? RequestedTelegramUserId { get; private set; }
        public bool FailLoads { get; set; }
        public Exception? LoadFailure { get; init; }
        public Exception? PurchaseFailure { get; init; }
        public int PurchaseCalls { get; private set; }
        public TaskCompletionSource<PurchaseServiceResponse?>? PendingPurchase { get; init; }

        public Task<UserServicesResponse> GetUserServicesAsync(
            long telegramUserId,
            CancellationToken cancellationToken = default)
        {
            if (LoadFailure is not null)
            {
                return Task.FromException<UserServicesResponse>(LoadFailure);
            }

            if (FailLoads)
            {
                return Task.FromException<UserServicesResponse>(new HttpRequestException());
            }

            RequestedTelegramUserId = telegramUserId;
            var response = new UserServicesResponse(
                responseUserId ?? telegramUserId,
                12m,
                "EUR",
                [new ConnectedServiceDto(
                    "vpn",
                    "VPN",
                    "Protected connection",
                    ConnectedServiceStatus.Active,
                    "active")]);
            return Task.FromResult(response);
        }

        public Task<ServiceCatalogResponse> GetCatalogAsync(
            CancellationToken cancellationToken = default)
        {
            if (LoadFailure is not null)
            {
                return Task.FromException<ServiceCatalogResponse>(LoadFailure);
            }

            return FailLoads
                ? Task.FromException<ServiceCatalogResponse>(new HttpRequestException())
                : Task.FromResult(new ServiceCatalogResponse([]));
        }

        public Task<ProductOfferDto?> GetProductAsync(
            string productId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProductOfferDto?>(null);

        public Task<PurchaseServiceResponse?> PurchaseAsync(
            long telegramUserId,
            string productId,
            CancellationToken cancellationToken = default)
        {
            PurchaseCalls++;
            if (PurchaseFailure is not null)
            {
                return Task.FromException<PurchaseServiceResponse?>(PurchaseFailure);
            }

            return PendingPurchase?.Task ?? Task.FromResult<PurchaseServiceResponse?>(
                CompletedPurchase(telegramUserId, productId));
        }

        public PurchaseServiceResponse CompletedPurchase(long telegramUserId, string productId) =>
            new(
                telegramUserId,
                PurchaseServiceStatus.Completed,
                new ConnectedServiceDto(
                    productId,
                    "eSIM",
                    "Data and voice eSIM",
                    ConnectedServiceStatus.Active,
                    "active"),
                9m,
                3m,
                "EUR");
    }
}
