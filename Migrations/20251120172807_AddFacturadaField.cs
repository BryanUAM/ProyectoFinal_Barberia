using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaLosHermanos.Migrations
{
    /// <inheritdoc />
    public partial class AddFacturadaField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Facturada",
                table: "Appointment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facturada",
                table: "Appointment");
        }
    }
}
