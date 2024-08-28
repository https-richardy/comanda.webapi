using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comanda.WebApi.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class NewEntityUnselectedIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Additionals_Products_ProductId",
                table: "Additionals");

            migrationBuilder.DropIndex(
                name: "IX_Additionals_ProductId",
                table: "Additionals");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Additionals");

            migrationBuilder.CreateTable(
                name: "UnselectedIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    CartItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnselectedIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnselectedIngredients_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnselectedIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnselectedIngredients_CartItemId",
                table: "UnselectedIngredients",
                column: "CartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UnselectedIngredients_IngredientId",
                table: "UnselectedIngredients",
                column: "IngredientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnselectedIngredients");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Additionals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Additionals_ProductId",
                table: "Additionals",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Additionals_Products_ProductId",
                table: "Additionals",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
