using System.ComponentModel.DataAnnotations;

namespace DespachoApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Operador"; // "Admin" o "Operador"

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
