using Microsoft.EntityFrameworkCore.Migrations;

namespace WalletKeeper.Persistence.Migrations
{
    public partial class Migration13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VAT",
                table: "ProductItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VAT",
                table: "ProductItems");
        }
    }
}
