using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BarberiaLosHermanos.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }

        [Required(ErrorMessage ="El nombre del producto es obligatorio.")]
        public string Nombre { get; set; }
        public decimal Precio { get; set; }

        public bool Disponible { get; set; } = true;

        [ValidateNever]
        public virtual ICollection<ProductosFactura> ProductosFacturas { get; set; } = new List<ProductosFactura>();
    }
}

