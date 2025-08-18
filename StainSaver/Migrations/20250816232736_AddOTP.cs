using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class AddOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickedUpItemImage",
                table: "PickUps");

            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "PickUps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTP",
                table: "PickUps");

            migrationBuilder.AddColumn<string>(
                name: "PickedUpItemImage",
                table: "PickUps",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
