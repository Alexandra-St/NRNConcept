using Microsoft.EntityFrameworkCore;
using NRN.Telegram.Features.Services.Contracts;

namespace NRN.Telegram.Features.Services.Persistence;

public static class NrnDatabaseInitializer
{
    private const long PreviewUserId = 0;

    // TODO(real-data): Preview user 0 and its balance/subscription exist only for the
    // public portfolio demo. Never map this user to a production Telegram account.

    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var factory = scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<NrnServicesDbContext>>();
        await using var database = await factory.CreateDbContextAsync(cancellationToken);

        await database.Database.MigrateAsync(cancellationToken);

        if (!await database.Products.AnyAsync(cancellationToken))
        {
            database.Products.AddRange(CreateProducts());
            await database.SaveChangesAsync(cancellationToken);
        }

        if (!await database.UserSubscriptions
                .AnyAsync(item => item.TelegramUserId == PreviewUserId, cancellationToken))
        {
            database.UserSubscriptions.AddRange(
                new UserSubscriptionEntity
                {
                    TelegramUserId = PreviewUserId,
                    ProductId = "esim",
                    Status = ConnectedServiceStatus.Active,
                    StatusDetail = "баланс 0 EUR"
                });
            await database.SaveChangesAsync(cancellationToken);
        }

        if (!await database.UserAccounts
                .AnyAsync(item => item.TelegramUserId == PreviewUserId, cancellationToken))
        {
            database.UserAccounts.Add(new UserAccountEntity
            {
                TelegramUserId = PreviewUserId,
                BalanceMinor = 1200,
                Currency = "EUR"
            });
            await database.SaveChangesAsync(cancellationToken);
        }
    }

    private static ProductEntity[] CreateProducts() =>
    [
        new()
        {
            Id = "virtual-numbers",
            Name = "Virtual Numbers",
            Description = "Mobile and toll-free numbers with voice and SMS support in more than 20 countries.",
            ConnectionFeeMinor = 0,
            MonthlyPriceMinor = 1000,
            Currency = "EUR",
            Kind = ServicePlanKind.VirtualNumber,
            FlagEmoji = "☎️",
            IncludesPhoneNumber = true,
            Accent = "virtual",
            MonthlyPriceIsFrom = true
        },
        new()
        {
            Id = "esim",
            Name = "eSIM",
            Description = "Data and voice eSIM with instant digital delivery and no physical card.",
            ConnectionFeeMinor = 900,
            MonthlyPriceMinor = 0,
            Currency = "EUR",
            Kind = ServicePlanKind.Esim,
            FlagEmoji = "🌐",
            IncludesPhoneNumber = false,
            Accent = "esim",
            IsPopular = true,
            ConnectionPriceIsFrom = true
        },
        new()
        {
            Id = "physical-sim",
            Name = "Physical SIM",
            Description = "A full-featured physical SIM with data, calls and SMS. Delivery is charged separately.",
            ConnectionFeeMinor = 1400,
            MonthlyPriceMinor = 0,
            Currency = "EUR",
            Kind = ServicePlanKind.PhysicalSim,
            FlagEmoji = "📱",
            IncludesPhoneNumber = true,
            Accent = "physical"
        }
    ];
}
