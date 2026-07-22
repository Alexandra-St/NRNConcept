using Microsoft.EntityFrameworkCore;
using NRN.Telegram.Features.Payments.Persistence;
using NRN.Telegram.Features.Services.Contracts;
using NRN.Telegram.Features.Services.Persistence;

namespace NRN.Telegram.Features.Payments.Store;

public sealed class SqliteTelegramStarsPaymentStore(
    IDbContextFactory<NrnServicesDbContext> contextFactory,
    TimeProvider timeProvider) : ITelegramStarsPaymentStore
{
    public async Task<PendingStarsInvoice?> CreatePendingAsync(
        long telegramUserId,
        string productId,
        int starsAmount,
        CancellationToken cancellationToken = default)
    {
        if (starsAmount <= 0)
        {
            return null;
        }

        await using var database = await contextFactory.CreateDbContextAsync(cancellationToken);
        var product = await database.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);
        if (product is null || await database.UserSubscriptions.AnyAsync(
                item => item.TelegramUserId == telegramUserId && item.ProductId == productId,
                cancellationToken))
        {
            return null;
        }

        var payload = $"nrn:{Guid.NewGuid():N}";
        database.StarsPayments.Add(new StarsPaymentEntity
        {
            Id = Guid.NewGuid(),
            InvoicePayload = payload,
            TelegramUserId = telegramUserId,
            ProductId = productId,
            StarsAmount = starsAmount,
            Status = StarsPaymentStatus.Pending,
            CreatedAtUtc = timeProvider.GetUtcNow()
        });
        await database.SaveChangesAsync(cancellationToken);

        return new PendingStarsInvoice(payload, product.Name, product.Description, starsAmount);
    }

    public async Task<bool> CanCheckoutAsync(
        string payload,
        long telegramUserId,
        string currency,
        int totalAmount,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(currency, "XTR", StringComparison.Ordinal))
        {
            return false;
        }

        await using var database = await contextFactory.CreateDbContextAsync(cancellationToken);
        var payment = await database.StarsPayments
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.InvoicePayload == payload, cancellationToken);
        if (payment is null ||
            payment.Status != StarsPaymentStatus.Pending ||
            payment.TelegramUserId != telegramUserId ||
            payment.StarsAmount != totalAmount)
        {
            return false;
        }

        return !await database.UserSubscriptions.AnyAsync(
            item => item.TelegramUserId == telegramUserId && item.ProductId == payment.ProductId,
            cancellationToken);
    }

    public async Task<ConnectedServiceDto?> CompleteAsync(
        string payload,
        long telegramUserId,
        string currency,
        int totalAmount,
        string telegramChargeId,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(currency, "XTR", StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(telegramChargeId))
        {
            return null;
        }

        await using var database = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        var payment = await database.StarsPayments
            .SingleOrDefaultAsync(item => item.InvoicePayload == payload, cancellationToken);
        if (payment is null ||
            payment.TelegramUserId != telegramUserId ||
            payment.StarsAmount != totalAmount)
        {
            return null;
        }

        var product = await database.Products
            .SingleAsync(item => item.Id == payment.ProductId, cancellationToken);
        var subscription = await database.UserSubscriptions.SingleOrDefaultAsync(
            item => item.TelegramUserId == telegramUserId && item.ProductId == payment.ProductId,
            cancellationToken);

        if (payment.Status == StarsPaymentStatus.Pending)
        {
            payment.Status = StarsPaymentStatus.Paid;
            payment.TelegramChargeId = telegramChargeId;
            payment.CompletedAtUtc = timeProvider.GetUtcNow();
            if (subscription is null)
            {
                subscription = new UserSubscriptionEntity
                {
                    TelegramUserId = telegramUserId,
                    ProductId = payment.ProductId,
                    Status = ConnectedServiceStatus.Active,
                    StatusDetail = $"оплачено {totalAmount} Stars"
                };
                database.UserSubscriptions.Add(subscription);
            }

            await database.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        else if (!string.Equals(payment.TelegramChargeId, telegramChargeId, StringComparison.Ordinal))
        {
            return null;
        }

        if (subscription is null)
        {
            return null;
        }

        return new ConnectedServiceDto(
            product.Id,
            product.Name,
            product.Description,
            subscription.Status,
            subscription.StatusDetail);
    }
}
