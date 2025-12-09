using System.ComponentModel.DataAnnotations;

namespace BarberiaLosHermanos.Models
{
    public class Barbero
    {
        [Key]
        public int BarberoId { get; set; }

        [Required]
        public string Nombre { get; set; }

        public bool Activo { get; set; } = true;

    }
}
