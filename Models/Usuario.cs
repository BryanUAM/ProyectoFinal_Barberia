using System.ComponentModel.DataAnnotations;

namespace BarberiaLosHermanos.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Contrasena { get; set; }

        [StringLength(12)]
        public string? Telefono { get; set; }


        [Required]
        public string Rol { get; set; }  
    }
}
