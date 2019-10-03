using Microsoft.EntityFrameworkCore.Migrations;

namespace IOTCashReader.Migrations
{
    public partial class updateReqTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Counts",
                table: "Request",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Counts",
                table: "Request");
        }
    }
}
