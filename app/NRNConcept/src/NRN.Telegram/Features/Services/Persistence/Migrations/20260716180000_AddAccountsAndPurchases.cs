using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRN.Telegram.Features.Services.Persistence.Migrations;

[DbContext(typeof(NrnServicesDbContext))]
[Migration("20260716180000_AddAccountsAndPurchases")]
public partial class AddAccountsAndPurchases : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserAccounts",
            columns: table => new
            {
                TelegramUserId = table.Column<long>(type: "INTEGER", nullable: false),
                BalanceMinor = table.Column<int>(type: "INTEGER", nullable: false),
                Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_UserAccounts", item => item.TelegramUserId));

        migrationBuilder.CreateTable(
            name: "Purchases",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                TelegramUserId = table.Column<long>(type: "INTEGER", nullable: false),
                ProductId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                AmountMinor = table.Column<int>(type: "INTEGER", nullable: false),
                Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Purchases", item => item.Id);
                table.ForeignKey(
                    name: "FK_Purchases_Products_ProductId",
                    column: item => item.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Purchases_ProductId",
            table: "Purchases",
            column: "ProductId");
        migrationBuilder.CreateIndex(
            name: "IX_Purchases_TelegramUserId_CreatedAtUtc",
            table: "Purchases",
            columns: ["TelegramUserId", "CreatedAtUtc"]);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Purchases");
        migrationBuilder.DropTable(name: "UserAccounts");
    }
}
