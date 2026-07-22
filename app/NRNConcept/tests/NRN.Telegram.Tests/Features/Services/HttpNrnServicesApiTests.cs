using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using NRN.Telegram.Features.Services.Api;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Telegram;

namespace NRN.Telegram.Tests.Features.Services;

public sealed class HttpNrnServicesApiTests
{
    [Fact]
    public async Task GetUserServicesAsync_SendsValidatedInitDataToCurrentUserEndpoint()
    {
        var handler = new RecordingHandler();
        var session = AuthenticatedSession(4821, "signed-init-data");
        var api = new HttpNrnServicesApi(
            new HttpClient(handler),
            new TestNavigationManager(),
            session);

        var response = await api.GetUserServicesAsync(
            4821,
            TestContext.Current.CancellationToken);

        Assert.Equal(4821, response.TelegramUserId);
        Assert.Equal("https://nrn.test/api/v1/services", handler.RequestUri?.ToString());
        Assert.Equal("signed-init-data", handler.InitDataHeader);
    }

    [Fact]
    public async Task GetUserServicesAsync_DoesNotSendRequest_ForAnotherTelegramUser()
    {
        var handler = new RecordingHandler();
        var session = AuthenticatedSession(4821, "signed-init-data");
        var api = new HttpNrnServicesApi(
            new HttpClient(handler),
            new TestNavigationManager(),
            session);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            api.GetUserServicesAsync(999, TestContext.Current.CancellationToken));

        Assert.Null(handler.RequestUri);
    }

    private static TelegramSessionState AuthenticatedSession(long userId, string initData)
    {
        var result = TelegramValidationResult.Success(
            new TelegramUser(userId, "Alex", null, null, "ru", null),
            "query");
        var session = new TelegramSessionState(new StubValidator(result));
        session.Initialize(initData);
        return session;
    }

    private sealed class StubValidator(TelegramValidationResult result) : ITelegramInitDataValidator
    {
        public TelegramValidationResult Validate(string? initData) => result;
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }
        public string? InitDataHeader { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            InitDataHeader = request.Headers.TryGetValues(TelegramApiHeaders.InitData, out var values)
                ? values.Single()
                : null;

            var payload = JsonSerializer.Serialize(
                new UserServicesResponse(4821, 0, "EUR", []),
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager() =>
            Initialize("https://nrn.test/", "https://nrn.test/");

        protected override void NavigateToCore(string uri, bool forceLoad) =>
            throw new NotSupportedException();
    }
}
