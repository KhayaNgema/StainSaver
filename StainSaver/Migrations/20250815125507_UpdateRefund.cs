using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RefundedAmount",
                table: "Refunds",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundedAmount",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Refunds");
        }
    }
}
