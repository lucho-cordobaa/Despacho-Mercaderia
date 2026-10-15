using System.ComponentModel.DataAnnotations;

namespace DespachoApp.Models
{
    public class Direccion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria")]
        public string Calle { get; set; } = string.Empty;

        [Required(ErrorMessage = "El numero es obligatorio")]
        [Display(Name = "Número")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La localidad es obligatoria")]
        public string Localidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La provincia es obligatoria")]
        public string Provincia { get; set; } = string.Empty;

        // FK
        [Required]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
    }
}
