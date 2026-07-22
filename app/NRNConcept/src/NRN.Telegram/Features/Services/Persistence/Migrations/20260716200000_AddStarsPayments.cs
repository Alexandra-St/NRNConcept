using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRN.Telegram.Features.Services.Persistence.Migrations;

[DbContext(typeof(NrnServicesDbContext))]
[Migration("20260716200000_AddStarsPayments")]
public partial class AddStarsPayments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "StarsPayments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                InvoicePayload = table.Column<string>(type: "TEXT", maxLength: 96, nullable: false),
                TelegramUserId = table.Column<long>(type: "INTEGER", nullable: false),
                ProductId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                StarsAmount = table.Column<int>(type: "INTEGER", nullable: false),
                Status = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                TelegramChargeId = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                CompletedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_StarsPayments", item => item.Id));

        migrationBuilder.CreateIndex(
            name: "IX_StarsPayments_InvoicePayload",
            table: "StarsPayments",
            column: "InvoicePayload",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_StarsPayments_TelegramChargeId",
            table: "StarsPayments",
            column: "TelegramChargeId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "StarsPayments");
}
