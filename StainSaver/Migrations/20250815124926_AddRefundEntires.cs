using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundEntires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundPolicies",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "RefundValidations",
                table: "Refunds");

            migrationBuilder.CreateTable(
                name: "RefundPolicyEntries",
                columns: table => new
                {
                    RefundPolicyEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundId = table.Column<int>(type: "int", nullable: false),
                    RefundPolicy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundPolicyEntries", x => x.RefundPolicyEntryId);
                    table.ForeignKey(
                        name: "FK_RefundPolicyEntries_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refunds",
                        principalColumn: "RefundId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RefundValidationEntries",
                columns: table => new
                {
                    RefundValidationEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundId = table.Column<int>(type: "int", nullable: false),
                    RefundValidation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundValidationEntries", x => x.RefundValidationEntryId);
                    table.ForeignKey(
                        name: "FK_RefundValidationEntries_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refunds",
                        principalColumn: "RefundId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefundPolicyEntries_RefundId",
                table: "RefundPolicyEntries",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundValidationEntries_RefundId",
                table: "RefundValidationEntries",
                column: "RefundId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefundPolicyEntries");

            migrationBuilder.DropTable(
                name: "RefundValidationEntries");

            migrationBuilder.AddColumn<string>(
                name: "RefundPolicies",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefundValidations",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
