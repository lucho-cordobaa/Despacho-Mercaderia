using System.ComponentModel.DataAnnotations;

namespace DespachoApp.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre / Razón social")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI/CUIT es obligatorio")]
        [Display(Name = "DNI / CUIT")]
        public string DniCuit { get; set; } = string.Empty;

        [RegularExpression(@"^\d{10}$", ErrorMessage = "El teléfono debe tener 10 dígitos.")]
        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El email no es valido")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
