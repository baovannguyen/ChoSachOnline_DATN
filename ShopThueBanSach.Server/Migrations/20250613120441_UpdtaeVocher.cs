using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdtaeVocher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredPoints",
                table: "Vouchers");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "DiscountCodes",
                newName: "RequiredPoints");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Vouchers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DiscountCodeId",
                table: "Vouchers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "DiscountCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_DiscountCodeId",
                table: "Vouchers",
                column: "DiscountCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_DiscountCodes_DiscountCodeId",
                table: "Vouchers",
                column: "DiscountCodeId",
                principalTable: "DiscountCodes",
                principalColumn: "DiscountCodeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_DiscountCodes_DiscountCodeId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_DiscountCodeId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "DiscountCodeId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "DiscountCodes");

            migrationBuilder.RenameColumn(
                name: "RequiredPoints",
                table: "DiscountCodes",
                newName: "Quantity");

            migrationBuilder.AddColumn<int>(
                name: "RequiredPoints",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
