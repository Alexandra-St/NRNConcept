using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRN.Telegram.Features.Services.Persistence.Migrations;

[DbContext(typeof(NrnServicesDbContext))]
[Migration("20260716120000_AlignCatalogWithBot")]
public partial class AlignCatalogWithBot : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ConnectionFeeMinor",
            table: "Products",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0);
        migrationBuilder.AddColumn<string>(
            name: "CountryCode",
            table: "Products",
            type: "TEXT",
            maxLength: 2,
            nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "FlagEmoji",
            table: "Products",
            type: "TEXT",
            maxLength: 12,
            nullable: false,
            defaultValue: "🌐");
        migrationBuilder.AddColumn<bool>(
            name: "IncludesPhoneNumber",
            table: "Products",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);
        migrationBuilder.AddColumn<string>(
            name: "Kind",
            table: "Products",
            type: "TEXT",
            maxLength: 24,
            nullable: false,
            defaultValue: "DataOnly");

        migrationBuilder.Sql("DELETE FROM UserSubscriptions; DELETE FROM Products;");

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

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DELETE FROM UserSubscriptions; DELETE FROM Products;");
        migrationBuilder.DropColumn(name: "ConnectionFeeMinor", table: "Products");
        migrationBuilder.DropColumn(name: "CountryCode", table: "Products");
        migrationBuilder.DropColumn(name: "FlagEmoji", table: "Products");
        migrationBuilder.DropColumn(name: "IncludesPhoneNumber", table: "Products");
        migrationBuilder.DropColumn(name: "Kind", table: "Products");

        migrationBuilder.Sql(
            """
            INSERT INTO Products (Id, Name, Description, MonthlyPriceMinor, Currency, Accent, IsPopular)
            VALUES
                ('vpn-protection', 'VPN Protection', 'Защищает соединение в публичных и домашних сетях', 499, 'EUR', 'vpn', 1),
                ('data-monitor', 'Data Monitor', 'Предупреждает, если ваши данные появились в утечке', 299, 'EUR', 'monitor', 0),
                ('privacy-bundle', 'Privacy Bundle', 'VPN, мониторинг утечек и семейная защита', 799, 'EUR', 'bundle', 0);
            """);
    }
}
