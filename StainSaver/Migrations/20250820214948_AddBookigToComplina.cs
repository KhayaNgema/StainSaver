using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class AddBookigToComplina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Complains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Complains_BookingId",
                table: "Complains",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complains_Bookings_BookingId",
                table: "Complains",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complains_Bookings_BookingId",
                table: "Complains");

            migrationBuilder.DropIndex(
                name: "IX_Complains_BookingId",
                table: "Complains");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Complains");
        }
    }
}
