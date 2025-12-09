using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaLosHermanos.Migrations
{
    /// <inheritdoc />
    public partial class MultiServiciosCita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Producto_ProductoId",
                table: "Appointment");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "Appointment",
                newName: "BarberoId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_ProductoId",
                table: "Appointment",
                newName: "IX_Appointment_BarberoId");

            migrationBuilder.CreateTable(
                name: "AppointmentServicio",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentServicio", x => new { x.AppointmentId, x.ProductoId });
                    table.ForeignKey(
                        name: "FK_AppointmentServicio_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentServicio_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Barbero",
                columns: table => new
                {
                    BarberoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barbero", x => x.BarberoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServicio_ProductoId",
                table: "AppointmentServicio",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Barbero_BarberoId",
                table: "Appointment",
                column: "BarberoId",
                principalTable: "Barbero",
                principalColumn: "BarberoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Barbero_BarberoId",
                table: "Appointment");

            migrationBuilder.DropTable(
                name: "AppointmentServicio");

            migrationBuilder.DropTable(
                name: "Barbero");

            migrationBuilder.RenameColumn(
                name: "BarberoId",
                table: "Appointment",
                newName: "ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_BarberoId",
                table: "Appointment",
                newName: "IX_Appointment_ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Producto_ProductoId",
                table: "Appointment",
                column: "ProductoId",
                principalTable: "Producto",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
