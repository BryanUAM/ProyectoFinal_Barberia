using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BarberiaLosHermanos.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un barbero.")]
        public int BarberoId { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Debe indicar la fecha y hora.")]
        public DateTime FechaHora { get; set; }

        public bool Realizada { get; set; } = false;

        // ⭐ NUEVO CAMPO
        public bool Facturada { get; set; } = false;

        [ValidateNever]
        public virtual Cliente? Cliente { get; set; }

        [ValidateNever]
        public virtual Barbero? Barbero { get; set; }

        [ValidateNever]
        public virtual ICollection<AppointmentServicio> Servicios { get; set; }
            = new List<AppointmentServicio>();
    }
}
