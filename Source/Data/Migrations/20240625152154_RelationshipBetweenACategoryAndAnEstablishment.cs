using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comanda.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipBetweenACategoryAndAnEstablishment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstablishmentId",
                table: "Categories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_EstablishmentId",
                table: "Categories",
                column: "EstablishmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Establishments_EstablishmentId",
                table: "Categories",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Establishments_EstablishmentId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_EstablishmentId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "EstablishmentId",
                table: "Categories");
        }
    }
}
