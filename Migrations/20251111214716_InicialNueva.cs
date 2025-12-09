using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaLosHermanos.Migrations
{
    /// <inheritdoc />
    public partial class InicialNueva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cliente",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "Servicio",
                table: "Appointment");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ClienteId",
                table: "Appointment",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ProductoId",
                table: "Appointment",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Cliente_ClienteId",
                table: "Appointment",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Producto_ProductoId",
                table: "Appointment",
                column: "ProductoId",
                principalTable: "Producto",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Cliente_ClienteId",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Producto_ProductoId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_ClienteId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_ProductoId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "Appointment");

            migrationBuilder.AddColumn<string>(
                name: "Cliente",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Servicio",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
