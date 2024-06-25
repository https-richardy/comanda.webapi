using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comanda.WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class FullNameInEstablishmentOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "EstablishmentOwners",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "EstablishmentOwners");
        }
    }
}
