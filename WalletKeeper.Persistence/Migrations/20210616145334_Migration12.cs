using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletKeeper.Persistence.Migrations
{
    public partial class Migration12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "ProductItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductItems_UserID",
                table: "ProductItems",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Users_UserID",
                table: "ProductItems",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Users_UserID",
                table: "ProductItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductItems_UserID",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "ProductItems");
        }
    }
}
