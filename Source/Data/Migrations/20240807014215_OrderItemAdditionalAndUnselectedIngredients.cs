using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comanda.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemAdditionalAndUnselectedIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "UnselectedIngredients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderItemAdditional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AdditionalId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemAdditional", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemAdditional_Additionals_AdditionalId",
                        column: x => x.AdditionalId,
                        principalTable: "Additionals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemAdditional_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnselectedIngredients_OrderItemId",
                table: "UnselectedIngredients",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAdditional_AdditionalId",
                table: "OrderItemAdditional",
                column: "AdditionalId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAdditional_OrderItemId",
                table: "OrderItemAdditional",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnselectedIngredients_OrderItems_OrderItemId",
                table: "UnselectedIngredients",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnselectedIngredients_OrderItems_OrderItemId",
                table: "UnselectedIngredients");

            migrationBuilder.DropTable(
                name: "OrderItemAdditional");

            migrationBuilder.DropIndex(
                name: "IX_UnselectedIngredients_OrderItemId",
                table: "UnselectedIngredients");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "UnselectedIngredients");
        }
    }
}
