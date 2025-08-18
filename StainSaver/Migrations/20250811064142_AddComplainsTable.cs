using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StainSaver.Migrations
{
    /// <inheritdoc />
    public partial class AddComplainsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Complains",
                columns: table => new
                {
                    ComplainId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ComplainType = table.Column<int>(type: "int", nullable: false),
                    BankAccountType = table.Column<int>(type: "int", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProofOfDamage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProofOfPayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLost = table.Column<bool>(type: "bit", nullable: true),
                    IsFound = table.Column<bool>(type: "bit", nullable: false),
                    ItemLostOrFound = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LostOrFoundDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageOfLostOrFoundItem = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complains", x => x.ComplainId);
                    table.ForeignKey(
                        name: "FK_Complains_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Complains_CustomerId",
                table: "Complains",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complains");
        }
    }
}
