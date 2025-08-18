using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryMethodToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryMethod",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "Bookings");
        }
    }
}
