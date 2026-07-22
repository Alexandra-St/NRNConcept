using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace NRN.Telegram.Features.Services.Persistence.Migrations;

[DbContext(typeof(NrnServicesDbContext))]
partial class NrnServicesDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "9.0.16");

        modelBuilder.Entity("NRN.Telegram.Features.Services.Persistence.ProductEntity", entity =>
        {
            entity.Property<string>("Id").HasMaxLength(80).HasColumnType("TEXT");
            entity.Property<string>("Accent").IsRequired().HasMaxLength(40).HasColumnType("TEXT");
            entity.Property<int>("ConnectionFeeMinor").HasColumnType("INTEGER");
            entity.Property<bool>("ConnectionPriceIsFrom").HasColumnType("INTEGER");
            entity.Property<string>("CountryCode").HasMaxLength(2).HasColumnType("TEXT");
            entity.Property<string>("Currency").IsRequired().HasMaxLength(3).HasColumnType("TEXT");
            entity.Property<string>("Description").IsRequired().HasMaxLength(600).HasColumnType("TEXT");
            entity.Property<string>("FlagEmoji").IsRequired().HasMaxLength(12).HasColumnType("TEXT");
            entity.Property<bool>("IncludesPhoneNumber").HasColumnType("INTEGER");
            entity.Property<bool>("IsPopular").HasColumnType("INTEGER");
            entity.Property<string>("Kind").IsRequired().HasMaxLength(24).HasColumnType("TEXT");
            entity.Property<int>("MonthlyPriceMinor").HasColumnType("INTEGER");
            entity.Property<bool>("MonthlyPriceIsFrom").HasColumnType("INTEGER");
            entity.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
            entity.HasKey("Id");
            entity.ToTable("Products");
        });

        modelBuilder.Entity("NRN.Telegram.Features.Services.Persistence.PurchaseEntity", entity =>
        {
            entity.Property<Guid>("Id").HasColumnType("TEXT");
            entity.Property<int>("AmountMinor").HasColumnType("INTEGER");
            entity.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("TEXT");
            entity.Property<string>("Currency").IsRequired().HasMaxLength(3).HasColumnType("TEXT");
            entity.Property<string>("ProductId").IsRequired().HasMaxLength(80).HasColumnType("TEXT");
            entity.Property<long>("TelegramUserId").HasColumnType("INTEGER");
            entity.HasKey("Id");
            entity.HasIndex("ProductId");
            entity.HasIndex("TelegramUserId", "CreatedAtUtc");
            entity.ToTable("Purchases");
        });

        modelBuilder.Entity("NRN.Telegram.Features.Services.Persistence.UserAccountEntity", entity =>
        {
            entity.Property<long>("TelegramUserId").ValueGeneratedNever().HasColumnType("INTEGER");
            entity.Property<int>("BalanceMinor").HasColumnType("INTEGER");
            entity.Property<string>("Currency").IsRequired().HasMaxLength(3).HasColumnType("TEXT");
            entity.HasKey("TelegramUserId");
            entity.ToTable("UserAccounts");
        });

        modelBuilder.Entity("NRN.Telegram.Features.Payments.Persistence.StarsPaymentEntity", entity =>
        {
            entity.Property<Guid>("Id").HasColumnType("TEXT");
            entity.Property<DateTimeOffset?>("CompletedAtUtc").HasColumnType("TEXT");
            entity.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("TEXT");
            entity.Property<string>("InvoicePayload").IsRequired().HasMaxLength(96).HasColumnType("TEXT");
            entity.Property<string>("ProductId").IsRequired().HasMaxLength(80).HasColumnType("TEXT");
            entity.Property<int>("StarsAmount").HasColumnType("INTEGER");
            entity.Property<string>("Status").IsRequired().HasMaxLength(16).HasColumnType("TEXT");
            entity.Property<string>("TelegramChargeId").HasMaxLength(160).HasColumnType("TEXT");
            entity.Property<long>("TelegramUserId").HasColumnType("INTEGER");
            entity.HasKey("Id");
            entity.HasIndex("InvoicePayload").IsUnique();
            entity.HasIndex("TelegramChargeId").IsUnique();
            entity.ToTable("StarsPayments");
        });

        modelBuilder.Entity("NRN.Telegram.Features.Services.Persistence.UserSubscriptionEntity", entity =>
        {
            entity.Property<long>("TelegramUserId").HasColumnType("INTEGER");
            entity.Property<string>("ProductId").HasMaxLength(80).HasColumnType("TEXT");
            entity.Property<string>("Status").IsRequired().HasMaxLength(24).HasColumnType("TEXT");
            entity.Property<string>("StatusDetail").IsRequired().HasMaxLength(160).HasColumnType("TEXT");
            entity.HasKey("TelegramUserId", "ProductId");
            entity.HasIndex("ProductId");
            entity.ToTable("UserSubscriptions");
        });

        modelBuilder.Entity("NRN.Telegram.Features.Services.Persistence.PurchaseEntity", entity =>
        {
            entity.HasOne("NRN.Telegram.Features.Services.Persistence.ProductEntity", "Product")
                .WithMany("Purchases")
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.Navigation("Product");
        });

        modelBuilder.Entity("NRN.Telegram.Features.Services.Persistence.UserSubscriptionEntity", entity =>
        {
            entity.HasOne("NRN.Telegram.Features.Services.Persistence.ProductEntity", "Product")
                .WithMany("Subscriptions")
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.Navigation("Product");
        });

        modelBuilder.Entity("NRN.Telegram.Features.Services.Persistence.ProductEntity", entity =>
        {
            entity.Navigation("Purchases");
            entity.Navigation("Subscriptions");
        });
    }
}
