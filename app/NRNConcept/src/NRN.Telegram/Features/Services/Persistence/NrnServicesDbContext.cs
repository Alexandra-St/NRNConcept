using Microsoft.EntityFrameworkCore;
using NRN.Telegram.Features.Payments.Persistence;

namespace NRN.Telegram.Features.Services.Persistence;

public sealed class NrnServicesDbContext(DbContextOptions<NrnServicesDbContext> options)
    : DbContext(options)
{
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<UserSubscriptionEntity> UserSubscriptions => Set<UserSubscriptionEntity>();
    public DbSet<UserAccountEntity> UserAccounts => Set<UserAccountEntity>();
    public DbSet<PurchaseEntity> Purchases => Set<PurchaseEntity>();
    public DbSet<StarsPaymentEntity> StarsPayments => Set<StarsPaymentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<ProductEntity>();
        product.ToTable("Products");
        product.HasKey(entity => entity.Id);
        product.Property(entity => entity.Id).HasMaxLength(80);
        product.Property(entity => entity.Name).HasMaxLength(120).IsRequired();
        product.Property(entity => entity.Description).HasMaxLength(600).IsRequired();
        product.Property(entity => entity.Currency).HasMaxLength(3).IsRequired();
        product.Property(entity => entity.Kind)
            .HasConversion<string>()
            .HasMaxLength(24);
        product.Property(entity => entity.CountryCode).HasMaxLength(2);
        product.Property(entity => entity.FlagEmoji).HasMaxLength(12).IsRequired();
        product.Property(entity => entity.Accent).HasMaxLength(40).IsRequired();

        var subscription = modelBuilder.Entity<UserSubscriptionEntity>();
        subscription.ToTable("UserSubscriptions");
        subscription.HasKey(entity => new { entity.TelegramUserId, entity.ProductId });
        subscription.Property(entity => entity.ProductId).HasMaxLength(80);
        subscription.Property(entity => entity.Status)
            .HasConversion<string>()
            .HasMaxLength(24);
        subscription.Property(entity => entity.StatusDetail).HasMaxLength(160).IsRequired();
        subscription.HasOne(entity => entity.Product)
            .WithMany(entity => entity.Subscriptions)
            .HasForeignKey(entity => entity.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        var account = modelBuilder.Entity<UserAccountEntity>();
        account.ToTable("UserAccounts");
        account.HasKey(entity => entity.TelegramUserId);
        account.Property(entity => entity.TelegramUserId).ValueGeneratedNever();
        account.Property(entity => entity.Currency).HasMaxLength(3).IsRequired();

        var purchase = modelBuilder.Entity<PurchaseEntity>();
        purchase.ToTable("Purchases");
        purchase.HasKey(entity => entity.Id);
        purchase.Property(entity => entity.ProductId).HasMaxLength(80).IsRequired();
        purchase.Property(entity => entity.Currency).HasMaxLength(3).IsRequired();
        purchase.HasIndex(entity => new { entity.TelegramUserId, entity.CreatedAtUtc });
        purchase.HasOne(entity => entity.Product)
            .WithMany(entity => entity.Purchases)
            .HasForeignKey(entity => entity.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        var starsPayment = modelBuilder.Entity<StarsPaymentEntity>();
        starsPayment.ToTable("StarsPayments");
        starsPayment.HasKey(entity => entity.Id);
        starsPayment.Property(entity => entity.InvoicePayload).HasMaxLength(96).IsRequired();
        starsPayment.HasIndex(entity => entity.InvoicePayload).IsUnique();
        starsPayment.Property(entity => entity.ProductId).HasMaxLength(80).IsRequired();
        starsPayment.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(16);
        starsPayment.Property(entity => entity.TelegramChargeId).HasMaxLength(160);
        starsPayment.HasIndex(entity => entity.TelegramChargeId).IsUnique();
    }
}
