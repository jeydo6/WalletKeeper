using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletKeeper.Persistence.Migrations
{
    public partial class Migration10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receipts_FiscalDriveNumber",
                table: "Receipts");

            migrationBuilder.AlterColumn<string>(
                name: "FiscalDocumentNumber",
                table: "Receipts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_FiscalDocumentNumber_FiscalDriveNumber",
                table: "Receipts",
                columns: new[] { "FiscalDocumentNumber", "FiscalDriveNumber" },
                unique: true,
                filter: "[FiscalDocumentNumber] IS NOT NULL AND [FiscalDriveNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receipts_FiscalDocumentNumber_FiscalDriveNumber",
                table: "Receipts");

            migrationBuilder.AlterColumn<string>(
                name: "FiscalDocumentNumber",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_FiscalDriveNumber",
                table: "Receipts",
                column: "FiscalDriveNumber",
                unique: true,
                filter: "[FiscalDriveNumber] IS NOT NULL");
        }
    }
}
