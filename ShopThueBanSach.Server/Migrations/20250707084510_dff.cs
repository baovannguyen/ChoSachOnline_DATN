using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class dff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderDetails_SaleBooks_SaleBookId",
                table: "SaleOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderDetails_SaleBookId",
                table: "SaleOrderDetails");

            migrationBuilder.DropColumn(
                name: "ImageUser",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "SaleBookId",
                table: "SaleOrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "SaleOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "SaleOrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "SaleOrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "ImageUser",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SaleBookId",
                table: "SaleOrderDetails",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderDetails_SaleBookId",
                table: "SaleOrderDetails",
                column: "SaleBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderDetails_SaleBooks_SaleBookId",
                table: "SaleOrderDetails",
                column: "SaleBookId",
                principalTable: "SaleBooks",
                principalColumn: "SaleBookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
