using Microsoft.EntityFrameworkCore.Migrations;

namespace IOTCashReader.Migrations
{
    public partial class updatedtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Safe_SafeId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Safe_User_UserId",
                table: "Safe");

            migrationBuilder.DropIndex(
                name: "IX_Safe_UserId",
                table: "Safe");

            migrationBuilder.DropIndex(
                name: "IX_Request_SafeId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Safe");

            migrationBuilder.DropColumn(
                name: "SafeId",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "isAdmin",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAdmin",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Safe",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SafeId",
                table: "Request",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Safe_UserId",
                table: "Safe",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_SafeId",
                table: "Request",
                column: "SafeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Safe_SafeId",
                table: "Request",
                column: "SafeId",
                principalTable: "Safe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Safe_User_UserId",
                table: "Safe",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
