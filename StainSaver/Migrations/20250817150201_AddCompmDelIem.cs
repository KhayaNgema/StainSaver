using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class AddCompmDelIem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryItems_Delivery_DeliveryId",
                table: "DeliveryItems");

            migrationBuilder.RenameColumn(
                name: "DeliveryId",
                table: "DeliveryItems",
                newName: "ComplainId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryItems_DeliveryId",
                table: "DeliveryItems",
                newName: "IX_DeliveryItems_ComplainId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryItems_Complains_ComplainId",
                table: "DeliveryItems",
                column: "ComplainId",
                principalTable: "Complains",
                principalColumn: "ComplainId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryItems_Complains_ComplainId",
                table: "DeliveryItems");

            migrationBuilder.RenameColumn(
                name: "ComplainId",
                table: "DeliveryItems",
                newName: "DeliveryId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryItems_ComplainId",
                table: "DeliveryItems",
                newName: "IX_DeliveryItems_DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryItems_Delivery_DeliveryId",
                table: "DeliveryItems",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "DeliveryId",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
