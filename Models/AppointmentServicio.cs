using BarberiaLosHermanos.Models;

public class AppointmentServicio
{
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; }

    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
}
