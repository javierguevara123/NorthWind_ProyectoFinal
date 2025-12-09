namespace NorthWind.Sales.Entities.Dtos.Products.GetProducts;

/// <summary>
/// DTO de solo lectura para representar un producto en listados.
/// Contiene información resumida del producto.
/// </summary>
public class ProductDto(
    int id,
    string name,
    short unitsInStock,
    decimal unitPrice)
{
    public int Id => id;
    public string Name => name;
    public short UnitsInStock => unitsInStock;
    public decimal UnitPrice => unitPrice;

    // Propiedades calculadas (opcional)
    public decimal TotalValue => unitsInStock * unitPrice;
    public bool IsLowStock => unitsInStock < 10;
}
