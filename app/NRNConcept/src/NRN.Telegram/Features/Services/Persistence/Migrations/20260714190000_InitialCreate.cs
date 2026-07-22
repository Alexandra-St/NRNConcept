using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace NRN.Telegram.Features.Services.Persistence.Migrations;

[DbContext(typeof(NrnServicesDbContext))]
[Migration("20260714190000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                MonthlyPriceMinor = table.Column<int>(type: "INTEGER", nullable: false),
                Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                Accent = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                IsPopular = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserSubscriptions",
            columns: table => new
            {
                TelegramUserId = table.Column<long>(type: "INTEGER", nullable: false),
                ProductId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                Status = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                StatusDetail = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_UserSubscriptions",
                    x => new { x.TelegramUserId, x.ProductId });
                table.ForeignKey(
                    name: "FK_UserSubscriptions_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserSubscriptions_ProductId",
            table: "UserSubscriptions",
            column: "ProductId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserSubscriptions");
        migrationBuilder.DropTable(name: "Products");
    }
}
