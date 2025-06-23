using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionToRentBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromotionId",
                table: "RentBooks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentBooks_PromotionId",
                table: "RentBooks",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentBooks_Promotions_PromotionId",
                table: "RentBooks",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentBooks_Promotions_PromotionId",
                table: "RentBooks");

            migrationBuilder.DropIndex(
                name: "IX_RentBooks_PromotionId",
                table: "RentBooks");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "RentBooks");
        }
    }
}
