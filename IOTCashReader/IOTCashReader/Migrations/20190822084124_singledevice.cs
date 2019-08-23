using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IOTCashReader.Migrations
{
    public partial class singledevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SafeCredit");

            migrationBuilder.DropTable(
                name: "SafeWithdrawal");

            migrationBuilder.CreateTable(
                name: "UserCredit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    CreditId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredit_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCredit_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserWithdrawal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    WithdrawalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWithdrawal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWithdrawal_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserWithdrawal_Withdrawal_WithdrawalId",
                        column: x => x.WithdrawalId,
                        principalTable: "Withdrawal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCredit_CreditId",
                table: "UserCredit",
                column: "CreditId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredit_UserId",
                table: "UserCredit",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWithdrawal_UserId",
                table: "UserWithdrawal",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWithdrawal_WithdrawalId",
                table: "UserWithdrawal",
                column: "WithdrawalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCredit");

            migrationBuilder.DropTable(
                name: "UserWithdrawal");

            migrationBuilder.CreateTable(
                name: "SafeCredit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreditId = table.Column<int>(nullable: true),
                    SafeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafeCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafeCredit_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SafeCredit_Safe_SafeId",
                        column: x => x.SafeId,
                        principalTable: "Safe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_SafeCredit_CreditId",
                table: "SafeCredit",
                column: "CreditId");

            migrationBuilder.CreateIndex(
                name: "IX_SafeCredit_SafeId",
                table: "SafeCredit",
                column: "SafeId");

            migrationBuilder.CreateIndex(
                name: "IX_SafeWithdrawal_SafeId",
                table: "SafeWithdrawal",
                column: "SafeId");

            migrationBuilder.CreateIndex(
                name: "IX_SafeWithdrawal_WithdrawalId",
                table: "SafeWithdrawal",
                column: "WithdrawalId");
        }
    }
}
