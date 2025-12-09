using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberiaLosHermanos.Models
{
    public class ProductosFactura
    {
        [Key]
        public int ProductosFacturaID { get; set; }

        [ForeignKey("Factura")]
        public int FacturaId { get; set; }

        [ForeignKey("Producto")]
        public int ProductoId { get; set; }

        public virtual Factura Factura { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
