using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class AddPackagesAndDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_AspNetUsers_DriverId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Complains_ComplainId",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deliveries",
                table: "Deliveries");

            migrationBuilder.RenameTable(
                name: "Deliveries",
                newName: "Delivery");

            migrationBuilder.RenameIndex(
                name: "IX_Deliveries_DriverId",
                table: "Delivery",
                newName: "IX_Delivery_DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Deliveries_ComplainId",
                table: "Delivery",
                newName: "IX_Delivery_ComplainId");

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "Delivery",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Delivery",
                table: "Delivery",
                column: "DeliveryId");

            migrationBuilder.CreateTable(
                name: "DeliveryItems",
                columns: table => new
                {
                    DeliveryItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryId = table.Column<int>(type: "int", nullable: false),
                    LostOrFoundItemId = table.Column<int>(type: "int", nullable: false),
                    IsPackaged = table.Column<bool>(type: "bit", nullable: false),
                    IsCollected = table.Column<bool>(type: "bit", nullable: false),
                    PackagedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CollectionAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryItems", x => x.DeliveryItemId);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_Delivery_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Delivery",
                        principalColumn: "DeliveryId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DeliveryItems_LostOrFoundItems_LostOrFoundItemId",
                        column: x => x.LostOrFoundItemId,
                        principalTable: "LostOrFoundItems",
                        principalColumn: "LostOrFoundItemId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ComplainId = table.Column<int>(type: "int", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PackageNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QrCodeImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.PackageId);
                    table.ForeignKey(
                        name: "FK_Packages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Packages_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Packages_Complains_ComplainId",
                        column: x => x.ComplainId,
                        principalTable: "Complains",
                        principalColumn: "ComplainId");
                    table.ForeignKey(
                        name: "FK_Packages_Delivery_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Delivery",
                        principalColumn: "DeliveryId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_DeliveryId",
                table: "DeliveryItems",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryItems_LostOrFoundItemId",
                table: "DeliveryItems",
                column: "LostOrFoundItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ComplainId",
                table: "Packages",
                column: "ComplainId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CreatedById",
                table: "Packages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DeliveryId",
                table: "Packages",
                column: "DeliveryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DriverId",
                table: "Packages",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_AspNetUsers_DriverId",
                table: "Delivery",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Complains_ComplainId",
                table: "Delivery",
                column: "ComplainId",
                principalTable: "Complains",
                principalColumn: "ComplainId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_AspNetUsers_DriverId",
                table: "Delivery");

            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Complains_ComplainId",
                table: "Delivery");

            migrationBuilder.DropTable(
                name: "DeliveryItems");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Delivery",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Delivery");

            migrationBuilder.RenameTable(
                name: "Delivery",
                newName: "Deliveries");

            migrationBuilder.RenameIndex(
                name: "IX_Delivery_DriverId",
                table: "Deliveries",
                newName: "IX_Deliveries_DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Delivery_ComplainId",
                table: "Deliveries",
                newName: "IX_Deliveries_ComplainId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deliveries",
                table: "Deliveries",
                column: "DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_AspNetUsers_DriverId",
                table: "Deliveries",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Complains_ComplainId",
                table: "Deliveries",
                column: "ComplainId",
                principalTable: "Complains",
                principalColumn: "ComplainId",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
