using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletKeeper.Persistence.Migrations
{
    public partial class MIgration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "Receipts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_UserID",
                table: "Receipts",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Users_UserID",
                table: "Receipts",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Users_UserID",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_UserID",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Receipts");
        }
    }
}
