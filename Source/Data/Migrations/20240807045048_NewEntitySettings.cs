using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comanda.WebApi.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class NewEntitySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcceptAutomatically = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MaxConcurrentAutomaticOrders = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    EstimatedDeliveryTimeInMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0.0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "EstimatedDeliveryTimeInMinutes", "MaxConcurrentAutomaticOrders" },
                values: new object[] { 1, 30, 5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
