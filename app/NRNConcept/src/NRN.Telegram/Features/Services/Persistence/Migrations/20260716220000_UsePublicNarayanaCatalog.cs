using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRN.Telegram.Features.Services.Persistence.Migrations;

[DbContext(typeof(NrnServicesDbContext))]
[Migration("20260716220000_UsePublicNarayanaCatalog")]
public sealed class UsePublicNarayanaCatalog : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "ConnectionPriceIsFrom",
            table: "Products",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);
        migrationBuilder.AddColumn<bool>(
            name: "MonthlyPriceIsFrom",
            table: "Products",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);

        // TODO(real-data): This clears only the local MVP/demo ledger because the old bot
        // screenshot products have no verified mapping to the public 2026 catalog.
        // Replace with an authoritative backend migration before any production rollout.
        migrationBuilder.Sql("DELETE FROM StarsPayments; DELETE FROM Purchases; DELETE FROM UserSubscriptions; DELETE FROM Products;");
        migrationBuilder.Sql(
            """
            INSERT INTO Products
                (Id, Name, Description, ConnectionFeeMinor, MonthlyPriceMinor, Currency, Kind,
                 CountryCode, FlagEmoji, IncludesPhoneNumber, Accent, IsPopular,
                 ConnectionPriceIsFrom, MonthlyPriceIsFrom)
            VALUES
                ('virtual-numbers', 'Virtual Numbers',
                 'Mobile and toll-free numbers with voice and SMS support in more than 20 countries.',
                 0, 1000, 'EUR', 'VirtualNumber', NULL, '☎️', 1, 'virtual', 0, 0, 1),
                ('esim', 'eSIM',
                 'Data and voice eSIM with instant digital delivery and no physical card.',
                 900, 0, 'EUR', 'Esim', NULL, '🌐', 0, 'esim', 1, 1, 0),
                ('physical-sim', 'Physical SIM',
                 'A full-featured physical SIM with data, calls and SMS. Delivery is charged separately.',
                 1400, 0, 'EUR', 'PhysicalSim', NULL, '📱', 1, 'physical', 0, 0, 0);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM StarsPayments; DELETE FROM Purchases; DELETE FROM UserSubscriptions; DELETE FROM Products;");
        migrationBuilder.DropColumn(name: "ConnectionPriceIsFrom", table: "Products");
        migrationBuilder.DropColumn(name: "MonthlyPriceIsFrom", table: "Products");
        migrationBuilder.Sql(
            """
            INSERT INTO Products
                (Id, Name, Description, ConnectionFeeMinor, MonthlyPriceMinor, Currency, Kind,
                 CountryCode, FlagEmoji, IncludesPhoneNumber, Accent, IsPopular)
            VALUES
                ('data-only', 'Data Only',
                 'eSIM для мобильного интернета без номера телефона. Работает более чем в 100 странах.',
                 400, 0, 'EUR', 'DataOnly', NULL, '🌐', 0, 'data', 1),
                ('estonia-mobile', 'Estonia Mobile',
                 'Мобильный номер Эстонии с поддержкой SMS и звонков.',
                 400, 500, 'EUR', 'MobileNumber', 'EE', '🇪🇪', 1, 'estonia', 0),
                ('latvia-mobile', 'Latvia Mobile',
                 'Мобильный номер Латвии с поддержкой SMS и звонков.',
                 400, 500, 'EUR', 'MobileNumber', 'LV', '🇱🇻', 1, 'latvia', 0);
            """);
    }
}
