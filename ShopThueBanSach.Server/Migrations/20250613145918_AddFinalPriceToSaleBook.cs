using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalPriceToSaleBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "SaleBooks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "SaleBooks");
        }
    }
}
