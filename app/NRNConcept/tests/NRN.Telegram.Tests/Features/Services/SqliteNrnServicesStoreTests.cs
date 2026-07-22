using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Features.Services.Persistence;
using NRN.Telegram.Features.Services.Store;

namespace NRN.Telegram.Tests.Features.Services;

public sealed class SqliteNrnServicesStoreTests
{
    [Fact]
    public async Task GetUserServicesAsync_ReturnsOnlyRequestedUsersSubscriptions()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        var factory = new TestDbContextFactory(options);

        await using (var database = await factory.CreateDbContextAsync(
                         TestContext.Current.CancellationToken))
        {
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
                IsPopular = true
            });
            database.UserSubscriptions.AddRange(
                new UserSubscriptionEntity
                {
                    TelegramUserId = 10,
                    ProductId = "esim",
                    Status = ConnectedServiceStatus.Active,
                    StatusDetail = "active"
                },
                new UserSubscriptionEntity
                {
                    TelegramUserId = 20,
                    ProductId = "esim",
                    Status = ConnectedServiceStatus.Paused,
                    StatusDetail = "paused"
                });
            await database.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var store = new SqliteNrnServicesStore(factory, TimeProvider.System);
        var response = await store.GetUserServicesAsync(
            10,
            TestContext.Current.CancellationToken);

        var service = Assert.Single(response.Services);
        Assert.Equal(10, response.TelegramUserId);
        Assert.Equal(ConnectedServiceStatus.Active, service.Status);
        Assert.Equal("eSIM", service.Name);
    }

    [Fact]
    public async Task GetCatalogAsync_ConvertsMinorUnitsToPrice()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        var factory = new TestDbContextFactory(options);

        await using (var database = await factory.CreateDbContextAsync(
                         TestContext.Current.CancellationToken))
        {
            await database.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
            database.Products.Add(new ProductEntity
            {
                Id = "virtual-numbers",
                Name = "Virtual Numbers",
                Description = "Voice and SMS numbers",
                ConnectionFeeMinor = 0,
                MonthlyPriceMinor = 1000,
                Currency = "EUR",
                Kind = ServicePlanKind.VirtualNumber,
                FlagEmoji = "☎️",
                Accent = "virtual",
                MonthlyPriceIsFrom = true
            });
            await database.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var store = new SqliteNrnServicesStore(factory, TimeProvider.System);
        var response = await store.GetCatalogAsync(TestContext.Current.CancellationToken);

        var offer = Assert.Single(response.Offers);
        Assert.Equal(10m, offer.MonthlyPrice);
        Assert.True(offer.MonthlyPriceIsFrom);
    }

    [Fact]
    public async Task PurchaseAsync_CreatesOnlyCurrentUsersSubscription_AndIsIdempotent()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        var factory = new TestDbContextFactory(options);

        await using (var database = await factory.CreateDbContextAsync(
                         TestContext.Current.CancellationToken))
        {
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
            database.UserAccounts.Add(new UserAccountEntity
            {
                TelegramUserId = 10,
                BalanceMinor = 1000,
                Currency = "EUR"
            });
            await database.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var store = new SqliteNrnServicesStore(factory, TimeProvider.System);
        var first = await store.PurchaseAsync(
            10,
            "esim",
            TestContext.Current.CancellationToken);
        var second = await store.PurchaseAsync(
            10,
            "esim",
            TestContext.Current.CancellationToken);
        var anotherUser = await store.GetUserServicesAsync(
            20,
            TestContext.Current.CancellationToken);

        Assert.Equal(PurchaseServiceStatus.Completed, first?.Status);
        Assert.Equal(1m, first?.Balance);
        Assert.Equal(PurchaseServiceStatus.AlreadyConnected, second?.Status);
        Assert.Equal(1m, second?.Balance);
        Assert.Empty(anotherUser.Services);

        await using var verification = await factory.CreateDbContextAsync(
            TestContext.Current.CancellationToken);
        Assert.Equal(1, await verification.UserSubscriptions.CountAsync(
            TestContext.Current.CancellationToken));
        Assert.Equal(1, await verification.Purchases.CountAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task PurchaseAsync_DoesNotCreateSubscription_WhenBalanceIsInsufficient()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        var factory = new TestDbContextFactory(options);

        await using (var database = await factory.CreateDbContextAsync(
                         TestContext.Current.CancellationToken))
        {
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
            database.UserAccounts.Add(new UserAccountEntity
            {
                TelegramUserId = 10,
                BalanceMinor = 300,
                Currency = "EUR"
            });
            await database.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var store = new SqliteNrnServicesStore(factory, TimeProvider.System);
        var result = await store.PurchaseAsync(
            10,
            "esim",
            TestContext.Current.CancellationToken);

        Assert.Equal(PurchaseServiceStatus.InsufficientBalance, result?.Status);
        Assert.Equal(3m, result?.Balance);

        await using var verification = await factory.CreateDbContextAsync(
            TestContext.Current.CancellationToken);
        Assert.Empty(await verification.UserSubscriptions.ToListAsync(
            TestContext.Current.CancellationToken));
        Assert.Empty(await verification.Purchases.ToListAsync(
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task PurchaseAsync_UsesFirstMonthlyCharge_WhenConnectionFeeIsZero()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        var factory = new TestDbContextFactory(options);

        await using (var database = await factory.CreateDbContextAsync(
                         TestContext.Current.CancellationToken))
        {
            await database.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
            database.Products.Add(new ProductEntity
            {
                Id = "virtual-numbers",
                Name = "Virtual Numbers",
                Description = "Voice and SMS numbers",
                ConnectionFeeMinor = 0,
                MonthlyPriceMinor = 1000,
                MonthlyPriceIsFrom = true,
                Currency = "EUR",
                Kind = ServicePlanKind.VirtualNumber,
                FlagEmoji = "☎️",
                Accent = "virtual"
            });
            database.UserAccounts.Add(new UserAccountEntity
            {
                TelegramUserId = 10,
                BalanceMinor = 1200,
                Currency = "EUR"
            });
            await database.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var store = new SqliteNrnServicesStore(factory, TimeProvider.System);
        var result = await store.PurchaseAsync(
            10,
            "virtual-numbers",
            TestContext.Current.CancellationToken);

        Assert.Equal(PurchaseServiceStatus.Completed, result?.Status);
        Assert.Equal(10m, result?.ChargedAmount);
        Assert.Equal(2m, result?.Balance);
    }

    private sealed class TestDbContextFactory(
        DbContextOptions<NrnServicesDbContext> options)
        : IDbContextFactory<NrnServicesDbContext>
    {
        public NrnServicesDbContext CreateDbContext() => new(options);

        public Task<NrnServicesDbContext> CreateDbContextAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }
}
