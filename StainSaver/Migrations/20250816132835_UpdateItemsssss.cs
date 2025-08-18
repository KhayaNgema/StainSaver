using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemsssss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageOfLostOrFoundItem",
                table: "Complains");

            migrationBuilder.DropColumn(
                name: "ItemLostOrFound",
                table: "Complains");

            migrationBuilder.DropColumn(
                name: "ProofOfDamage",
                table: "Complains");

            migrationBuilder.CreateTable(
                name: "LostOrFoundItems",
                columns: table => new
                {
                    LostOrFoundItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComplainId = table.Column<int>(type: "int", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LostOrFoundItems", x => x.LostOrFoundItemId);
                    table.ForeignKey(
                        name: "FK_LostOrFoundItems_Complains_ComplainId",
                        column: x => x.ComplainId,
                        principalTable: "Complains",
                        principalColumn: "ComplainId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RefundItems",
                columns: table => new
                {
                    RefundItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComplainId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundItems", x => x.RefundItemId);
                    table.ForeignKey(
                        name: "FK_RefundItems_Complains_ComplainId",
                        column: x => x.ComplainId,
                        principalTable: "Complains",
                        principalColumn: "ComplainId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LostOrFoundItems_ComplainId",
                table: "LostOrFoundItems",
                column: "ComplainId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundItems_ComplainId",
                table: "RefundItems",
                column: "ComplainId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LostOrFoundItems");

            migrationBuilder.DropTable(
                name: "RefundItems");

            migrationBuilder.AddColumn<string>(
                name: "ImageOfLostOrFoundItem",
                table: "Complains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemLostOrFound",
                table: "Complains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofOfDamage",
                table: "Complains",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
