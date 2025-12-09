using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BarberiaLosHermanos.Models
{
    public class Factura
    {
        [Key]
        public int FacturaId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        public int ClienteId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MontoTotal { get; set; }

        [ValidateNever]
        public virtual Cliente? Cliente { get; set; }

        [ValidateNever]
        public virtual ICollection<ProductosFactura> ProductosFacturas { get; set; } = new List<ProductosFactura>();

    }
}
