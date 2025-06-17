using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopThueBanSach.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIdToStaffId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityNotifications_AspNetUsers_UserId",
                table: "ActivityNotifications");

            migrationBuilder.DropIndex(
                name: "IX_ActivityNotifications_UserId",
                table: "ActivityNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ActivityNotifications");

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "ActivityNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityNotifications_StaffId",
                table: "ActivityNotifications",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityNotifications_Staffs_StaffId",
                table: "ActivityNotifications",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityNotifications_Staffs_StaffId",
                table: "ActivityNotifications");

            migrationBuilder.DropIndex(
                name: "IX_ActivityNotifications_StaffId",
                table: "ActivityNotifications");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "ActivityNotifications");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ActivityNotifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityNotifications_UserId",
                table: "ActivityNotifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityNotifications_AspNetUsers_UserId",
                table: "ActivityNotifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
