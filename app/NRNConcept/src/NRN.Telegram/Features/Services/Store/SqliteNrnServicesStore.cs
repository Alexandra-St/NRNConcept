using Microsoft.EntityFrameworkCore;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Features.Services.Persistence;

namespace NRN.Telegram.Features.Services.Store;

public sealed class SqliteNrnServicesStore(
    IDbContextFactory<NrnServicesDbContext> contextFactory,
    TimeProvider timeProvider) : INrnServicesStore
{
    // TODO(real-data): Replace this local account/subscription store with the existing
    // Narayana bot backend once its authenticated API contract is available.
    public async Task<UserServicesResponse> GetUserServicesAsync(
        long telegramUserId,
        CancellationToken cancellationToken = default)
    {
        await using var database = await contextFactory.CreateDbContextAsync(cancellationToken);
        var subscriptions = await database.UserSubscriptions
            .AsNoTracking()
            .Include(item => item.Product)
            .Where(item => item.TelegramUserId == telegramUserId)
            .OrderBy(item => item.Product.Name)
            .ToListAsync(cancellationToken);
        var account = await database.UserAccounts
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.TelegramUserId == telegramUserId, cancellationToken);

        var services = subscriptions
            .Select(item => new ConnectedServiceDto(
                item.ProductId,
                item.Product.Name,
                item.Product.Description,
                item.Status,
                item.StatusDetail))
            .ToArray();
        return new UserServicesResponse(
            telegramUserId,
            (account?.BalanceMinor ?? 0) / 100m,
            account?.Currency ?? "EUR",
            services);
    }

    public async Task<ServiceCatalogResponse> GetCatalogAsync(
        CancellationToken cancellationToken = default)
    {
        await using var database = await contextFactory.CreateDbContextAsync(cancellationToken);
        var products = await database.Products
            .AsNoTracking()
            .OrderByDescending(item => item.IsPopular)
            .ThenBy(item => item.MonthlyPriceMinor)
            .ToListAsync(cancellationToken);

        var offers = products
            .Select(ToDto)
            .ToArray();
        return new ServiceCatalogResponse(offers);
    }

    public async Task<ProductOfferDto?> GetProductAsync(
        string productId,
        CancellationToken cancellationToken = default)
    {
        await using var database = await contextFactory.CreateDbContextAsync(cancellationToken);
        var product = await database.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);
        return product is null ? null : ToDto(product);
    }

    public async Task<PurchaseServiceResponse?> PurchaseAsync(
        long telegramUserId,
        string productId,
        CancellationToken cancellationToken = default)
    {
        await using var database = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        var product = await database.Products
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var subscription = await database.UserSubscriptions
            .SingleOrDefaultAsync(
                item => item.TelegramUserId == telegramUserId && item.ProductId == productId,
                cancellationToken);
        var account = await database.UserAccounts
            .SingleOrDefaultAsync(item => item.TelegramUserId == telegramUserId, cancellationToken);
        var balanceMinor = account?.BalanceMinor ?? 0;

        if (subscription is not null)
        {
            return new PurchaseServiceResponse(
                telegramUserId,
                PurchaseServiceStatus.AlreadyConnected,
                ToServiceDto(product, subscription),
                0,
                balanceMinor / 100m,
                product.Currency);
        }

        var initialChargeMinor = InitialChargeMinor(product);
        if (account is null ||
            !string.Equals(account.Currency, product.Currency, StringComparison.OrdinalIgnoreCase) ||
            account.BalanceMinor < initialChargeMinor)
        {
            return new PurchaseServiceResponse(
                telegramUserId,
                PurchaseServiceStatus.InsufficientBalance,
                null,
                0,
                balanceMinor / 100m,
                product.Currency);
        }

        // TODO(real-data): This is a portfolio/demo transaction. Production checkout must
        // use the bot's authoritative pricing, balance and subscription lifecycle.
        account.BalanceMinor -= initialChargeMinor;
        subscription = new UserSubscriptionEntity
        {
            TelegramUserId = telegramUserId,
            ProductId = productId,
            Status = ConnectedServiceStatus.Active,
            StatusDetail = "баланс 0 EUR"
        };
        database.UserSubscriptions.Add(subscription);
        database.Purchases.Add(new PurchaseEntity
        {
            Id = Guid.NewGuid(),
            TelegramUserId = telegramUserId,
            ProductId = productId,
            AmountMinor = initialChargeMinor,
            Currency = product.Currency,
            CreatedAtUtc = timeProvider.GetUtcNow()
        });
        await database.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new PurchaseServiceResponse(
            telegramUserId,
            PurchaseServiceStatus.Completed,
            ToServiceDto(product, subscription),
            initialChargeMinor / 100m,
            account.BalanceMinor / 100m,
            product.Currency);
    }

    private static ConnectedServiceDto ToServiceDto(
        ProductEntity product,
        UserSubscriptionEntity subscription) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            subscription.Status,
            subscription.StatusDetail);

    private static ProductOfferDto ToDto(ProductEntity item) =>
        new(
            item.Id,
            item.Name,
            item.Description,
            item.ConnectionFeeMinor / 100m,
            item.MonthlyPriceMinor / 100m,
            item.Currency,
            item.Kind,
            item.CountryCode,
            item.FlagEmoji,
            item.IncludesPhoneNumber,
            item.Accent,
            item.IsPopular,
            item.ConnectionPriceIsFrom,
            item.MonthlyPriceIsFrom);

    private static int InitialChargeMinor(ProductEntity product) =>
        product.ConnectionFeeMinor > 0
            ? product.ConnectionFeeMinor
            : product.MonthlyPriceMinor;
}
