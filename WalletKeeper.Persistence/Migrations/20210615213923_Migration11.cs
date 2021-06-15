using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletKeeper.Persistence.Migrations
{
    public partial class Migration11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserID",
                table: "Products",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UserID",
                table: "Products",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UserID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UserID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Products");
        }
    }
}
