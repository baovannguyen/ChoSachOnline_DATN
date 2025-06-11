using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentCartItem");

            migrationBuilder.CreateTable(
                name: "CartItemRent",
                columns: table => new
                {
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RentalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    RentOrderOrderId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemRent", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_CartItemRent_RentOrders_RentOrderOrderId",
                        column: x => x.RentOrderOrderId,
                        principalTable: "RentOrders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_RentOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BookPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RentalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentOrderDetails_RentBooks_BookId",
                        column: x => x.BookId,
                        principalTable: "RentBooks",
                        principalColumn: "RentBookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentOrderDetails_RentOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RentOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItemRent_RentOrderOrderId",
                table: "CartItemRent",
                column: "RentOrderOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentOrderDetails_BookId",
                table: "RentOrderDetails",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_RentOrderDetails_OrderId",
                table: "RentOrderDetails",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItemRent");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RentOrderDetails");

            migrationBuilder.CreateTable(
                name: "RentCartItem",
                columns: table => new
                {
                    RentBookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HasShippingFee = table.Column<bool>(type: "bit", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricePerDay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RentOrderOrderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RentalDays = table.Column<int>(type: "int", nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
    }
}
