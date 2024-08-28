using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comanda.WebApi.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class ConfirmationTokenScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfirmationTokenId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfirmationToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationToken", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ConfirmationTokenId",
                table: "AspNetUsers",
                column: "ConfirmationTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ConfirmationToken_ConfirmationTokenId",
                table: "AspNetUsers",
                column: "ConfirmationTokenId",
                principalTable: "ConfirmationToken",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ConfirmationToken_ConfirmationTokenId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ConfirmationToken");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ConfirmationTokenId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ConfirmationTokenId",
                table: "AspNetUsers");
        }
    }
}
