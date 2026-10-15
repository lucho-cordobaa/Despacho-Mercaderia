namespace DespachoApp.Models
{
    // Se guarda en Session mientras el pedido está siendo armado
    public class PedidoViewModel
    {
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public List<LineaPedidoVM> Lineas { get; set; } = new();

        public int TotalProductos => Lineas.Sum(l => l.Cantidad);
        public decimal MontoTotal => Lineas.Sum(l => l.Subtotal);
    }

    public class LineaPedidoVM
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal => Cantidad * CostoUnitario;
    }
}
