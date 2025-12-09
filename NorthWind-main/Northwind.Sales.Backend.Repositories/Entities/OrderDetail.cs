
namespace NorthWind.Sales.Backend.Repositories.Entities;

public class OrderDetail
{
    public int OrderId { get; set; } // Para relacionar el detalle con la Orden
    public Order Order { get; set; } // Para que se haga referencia a la cabecera de la Orden
    public int ProductId { get; set; } // Id del Producto
    public decimal UnitPrice { get; set; }
    public short Quantity { get; set; }
    public Product Product { get; set; } = null!;
}
