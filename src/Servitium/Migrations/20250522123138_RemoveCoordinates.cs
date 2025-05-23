using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servitium.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coordinates_Latitude",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "Coordinates_Longitude",
                table: "ServiceProviders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Coordinates_Latitude",
                table: "ServiceProviders",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Coordinates_Longitude",
                table: "ServiceProviders",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
