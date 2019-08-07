using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IOTCashReader.Migrations
{
    public partial class whithdrawaltables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Withdrawal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<double>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafeWithdrawal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SafeId = table.Column<int>(nullable: true),
                    WithdrawalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafeWithdrawal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafeWithdrawal_Safe_SafeId",
                        column: x => x.SafeId,
                        principalTable: "Safe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SafeWithdrawal_Withdrawal_WithdrawalId",
                        column: x => x.WithdrawalId,
                        principalTable: "Withdrawal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SafeWithdrawal_SafeId",
                table: "SafeWithdrawal",
                column: "SafeId");

            migrationBuilder.CreateIndex(
                name: "IX_SafeWithdrawal_WithdrawalId",
                table: "SafeWithdrawal",
                column: "WithdrawalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SafeWithdrawal");

            migrationBuilder.DropTable(
                name: "Withdrawal");
        }
    }
}
