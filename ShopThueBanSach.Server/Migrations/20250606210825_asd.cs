using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class asd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RentOrders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RentalDays = table.Column<int>(type: "int", nullable: false),
                    HasShippingFee = table.Column<bool>(type: "bit", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentOrders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "RentCartItem",
                columns: table => new
                {
                    RentBookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RentalDays = table.Column<int>(type: "int", nullable: true),
                    HasShippingFee = table.Column<bool>(type: "bit", nullable: true),
                    RentOrderOrderId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentCartItem", x => x.RentBookId);
                    table.ForeignKey(
                        name: "FK_RentCartItem_RentOrders_RentOrderOrderId",
                        column: x => x.RentOrderOrderId,
                        principalTable: "RentOrders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RentCartItem_RentOrderOrderId",
                table: "RentCartItem",
                column: "RentOrderOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentCartItem");

            migrationBuilder.DropTable(
                name: "RentOrders");
        }
    }
}
