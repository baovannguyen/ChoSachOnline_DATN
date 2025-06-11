using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItemRent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartItemRent",
                columns: table => new
                {
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RentOrderOrderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RentalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_CartItemRent_RentOrderOrderId",
                table: "CartItemRent",
                column: "RentOrderOrderId");
        }
    }
}
