using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletKeeper.Persistence.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiscalDocumentNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FiscalDriveNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FiscalType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalSum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_FiscalDocumentNumber",
                table: "Receipts",
                column: "FiscalDocumentNumber",
                unique: true,
                filter: "[FiscalDocumentNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Receipts");
        }
    }
}
