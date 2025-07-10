using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class moiddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "SaleOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "SaleOrders");
        }
    }
}
