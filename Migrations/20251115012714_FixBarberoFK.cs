using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaLosHermanos.Migrations
{
    public partial class FixBarberoFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Crear barbero temporal si no existe
            migrationBuilder.Sql(@"
                IF NOT EXISTS(SELECT 1 FROM Barbero)
                BEGIN
                    INSERT INTO Barbero (Nombre, Activo)
                    VALUES ('Barbero Temporal', 1);
                END
            ");

            // 2️⃣ Obtener ID del barbero temporal (o el primero que exista)
            //    y asignarlo a todas las citas sin BarberoId
            migrationBuilder.Sql(@"
                DECLARE @BarberoTempId INT;
                SELECT TOP 1 @BarberoTempId = BarberoId FROM Barbero ORDER BY BarberoId;

                UPDATE Appointment
                SET BarberoId = @BarberoTempId
                WHERE BarberoId IS NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No revertimos nada porque no borra datos
        }
    }
}

