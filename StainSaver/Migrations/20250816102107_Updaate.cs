using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class Updaate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "PickUps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickedUpItemImage",
                table: "PickUps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "PickUps");

            migrationBuilder.DropColumn(
                name: "PickedUpItemImage",
                table: "PickUps");
        }
    }
}
