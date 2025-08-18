using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class DeleteDelieveryAndDriverFrmPackg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Delivery_DeliveryId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_DeliveryId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "QrCodeImage",
                table: "Packages",
                newName: "BarcodeImage");

            migrationBuilder.RenameColumn(
                name: "PackageNumber",
                table: "Packages",
                newName: "ReferenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_PackageId",
                table: "Delivery",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Packages_PackageId",
                table: "Delivery",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "PackageId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Packages_PackageId",
                table: "Delivery");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_PackageId",
                table: "Delivery");

            migrationBuilder.RenameColumn(
                name: "ReferenceNumber",
                table: "Packages",
                newName: "PackageNumber");

            migrationBuilder.RenameColumn(
                name: "BarcodeImage",
                table: "Packages",
                newName: "QrCodeImage");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Packages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DeliveryId",
                table: "Packages",
                column: "DeliveryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Delivery_DeliveryId",
                table: "Packages",
                column: "DeliveryId",
                principalTable: "Delivery",
                principalColumn: "DeliveryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
