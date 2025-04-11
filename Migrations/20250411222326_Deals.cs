using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab5.Migrations
{
    /// <inheritdoc />
    public partial class Deals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DealId",
                table: "FoodDeliveryService",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Deal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodDeliveryServiceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deal", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodDeliveryService_DealId",
                table: "FoodDeliveryService",
                column: "DealId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodDeliveryService_Deal_DealId",
                table: "FoodDeliveryService",
                column: "DealId",
                principalTable: "Deal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodDeliveryService_Deal_DealId",
                table: "FoodDeliveryService");

            migrationBuilder.DropTable(
                name: "Deal");

            migrationBuilder.DropIndex(
                name: "IX_FoodDeliveryService_DealId",
                table: "FoodDeliveryService");

            migrationBuilder.DropColumn(
                name: "DealId",
                table: "FoodDeliveryService");
        }
    }
}
