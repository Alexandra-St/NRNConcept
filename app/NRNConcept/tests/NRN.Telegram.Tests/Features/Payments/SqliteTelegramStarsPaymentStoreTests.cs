using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NRN.Telegram.Features.Payments.Persistence;
using NRN.Telegram.Features.Payments.Store;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Features.Services.Persistence;

namespace NRN.Telegram.Tests.Features.Payments;

public sealed class SqliteTelegramStarsPaymentStoreTests
{
    [Fact]
    public async Task CompleteAsync_ActivatesServiceOnce_ForMatchingTelegramPayment()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var factory = await CreateFactoryAsync(connection);
        var store = new SqliteTelegramStarsPaymentStore(factory, TimeProvider.System);

        var pending = await store.CreatePendingAsync(
            42,
            "esim",
            250,
            TestContext.Current.CancellationToken);

        Assert.NotNull(pending);
        Assert.True(await store.CanCheckoutAsync(
            pending.Payload,
            42,
            "XTR",
            250,
            TestContext.Current.CancellationToken));

        var first = await store.CompleteAsync(
            pending.Payload,
            42,
            "XTR",
            250,
            "charge-1",
            TestContext.Current.CancellationToken);
        var repeated = await store.CompleteAsync(
            pending.Payload,
            42,
            "XTR",
            250,
            "charge-1",
            TestContext.Current.CancellationToken);

        Assert.Equal("esim", first?.Id);
        Assert.Equal("esim", repeated?.Id);

        await using var database = await factory.CreateDbContextAsync(
            TestContext.Current.CancellationToken);
        Assert.Single(await database.UserSubscriptions.ToListAsync(
            TestContext.Current.CancellationToken));
        var payment = Assert.Single(await database.StarsPayments.ToListAsync(
            TestContext.Current.CancellationToken));
        Assert.Equal(StarsPaymentStatus.Paid, payment.Status);
        Assert.Equal("charge-1", payment.TelegramChargeId);
    }

    [Fact]
    public async Task CanCheckoutAsync_RejectsWrongUserAmountOrCurrency()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var factory = await CreateFactoryAsync(connection);
        var store = new SqliteTelegramStarsPaymentStore(factory, TimeProvider.System);
        var pending = await store.CreatePendingAsync(
            42,
            "esim",
            250,
            TestContext.Current.CancellationToken);

        Assert.NotNull(pending);
        Assert.False(await store.CanCheckoutAsync(
            pending.Payload, 99, "XTR", 250, TestContext.Current.CancellationToken));
        Assert.False(await store.CanCheckoutAsync(
            pending.Payload, 42, "EUR", 250, TestContext.Current.CancellationToken));
        Assert.False(await store.CanCheckoutAsync(
            pending.Payload, 42, "XTR", 249, TestContext.Current.CancellationToken));
    }

    private static async Task<TestDbContextFactory> CreateFactoryAsync(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        var factory = new TestDbContextFactory(options);
        await using var database = await factory.CreateDbContextAsync(
            TestContext.Current.CancellationToken);
        await database.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        database.Products.Add(new ProductEntity
        {
            Id = "esim",
            Name = "eSIM",
            Description = "Data and voice eSIM",
            ConnectionFeeMinor = 900,
            MonthlyPriceMinor = 0,
            Currency = "EUR",
            Kind = ServicePlanKind.Esim,
            FlagEmoji = "🌐",
            Accent = "esim",
            ConnectionPriceIsFrom = true
        });
        await database.SaveChangesAsync(TestContext.Current.CancellationToken);
        return factory;
    }

    private sealed class TestDbContextFactory(DbContextOptions<NrnServicesDbContext> options)
        : IDbContextFactory<NrnServicesDbContext>
    {
        public NrnServicesDbContext CreateDbContext() => new(options);

        public Task<NrnServicesDbContext> CreateDbContextAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }
}
