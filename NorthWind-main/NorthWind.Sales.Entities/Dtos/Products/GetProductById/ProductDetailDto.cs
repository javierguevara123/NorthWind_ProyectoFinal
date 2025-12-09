namespace NorthWind.Sales.Entities.Dtos.Products.GetProductById
{
    public record ProductDetailDto(
    int Id,
    string Name,
    short UnitsInStock,
    decimal UnitPrice
);
}
