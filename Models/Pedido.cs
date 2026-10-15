using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DespachoApp.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        public bool Confirmado { get; set; } = false;

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public decimal Total { get; set; }

        // FK Cliente
        [Required]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // FK Usuario
        [Required]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }

    public class DetallePedido
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }

        [Required]
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        // Se guarda el costo al momento del pedido (no cambia si cambia el producto)
        [Display(Name = "Costo unitario")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public decimal CostoUnitario { get; set; }

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal => Cantidad * CostoUnitario;
    }
}
