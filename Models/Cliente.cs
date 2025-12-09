using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BarberiaLosHermanos.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [StringLength(12)]
        public string? Telefono { get; set; } 
        [ValidateNever]
        public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}
