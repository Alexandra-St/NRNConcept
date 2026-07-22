using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NRN.Telegram.Features.Services.Persistence;

namespace NRN.Telegram.Tests.Features.Services;

public sealed class PublicCatalogMigrationTests
{
    [Fact]
    public async Task LatestMigrationCreatesVerifiedPublicCatalog()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var database = new NrnServicesDbContext(options);

        await database.Database.MigrateAsync(TestContext.Current.CancellationToken);
        var products = await database.Products
            .AsNoTracking()
            .OrderBy(item => item.Id)
            .ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, products.Count);
        var esim = Assert.Single(products, item => item.Id == "esim");
        Assert.Equal(900, esim.ConnectionFeeMinor);
        Assert.True(esim.ConnectionPriceIsFrom);
        var physicalSim = Assert.Single(products, item => item.Id == "physical-sim");
        Assert.Equal(1400, physicalSim.ConnectionFeeMinor);
        Assert.False(physicalSim.ConnectionPriceIsFrom);
        var virtualNumbers = Assert.Single(products, item => item.Id == "virtual-numbers");
        Assert.Equal(1000, virtualNumbers.MonthlyPriceMinor);
        Assert.True(virtualNumbers.MonthlyPriceIsFrom);
    }

    [Fact]
    public async Task LatestMigrationUpgradesPreviousLocalCatalog()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var database = new NrnServicesDbContext(options);
        var migrator = database.GetService<IMigrator>();

        await migrator.MigrateAsync(
            "20260716200000_AddStarsPayments",
            TestContext.Current.CancellationToken);
        database.UserAccounts.Add(new UserAccountEntity
        {
            TelegramUserId = 42,
            BalanceMinor = 500,
            Currency = "EUR"
        });
        database.UserSubscriptions.Add(new UserSubscriptionEntity
        {
            TelegramUserId = 42,
            ProductId = "data-only",
            Status = NRN.Telegram.Features.Services.Contracts.ConnectedServiceStatus.Active,
            StatusDetail = "demo"
        });
        await database.SaveChangesAsync(TestContext.Current.CancellationToken);

        await migrator.MigrateAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Empty(await database.UserSubscriptions.ToListAsync(
            TestContext.Current.CancellationToken));
        Assert.Equal(3, await database.Products.CountAsync(
            TestContext.Current.CancellationToken));
        Assert.NotNull(await database.Products.FindAsync(
            ["virtual-numbers"], TestContext.Current.CancellationToken));
    }
}
