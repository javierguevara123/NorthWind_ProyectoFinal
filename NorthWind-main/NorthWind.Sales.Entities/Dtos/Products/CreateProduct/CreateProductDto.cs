namespace NorthWind.Sales.Entities.Dtos.Products.CreateProduct
{
    public class CreateProductDto(
        string name,
        decimal unitPrice,
        short unitsInStock,
        string? profilePictureBase64) // <--- NUEVO CAMPO
    {
        public string Name => name;
        public decimal UnitPrice => unitPrice;
        public short UnitsInStock => unitsInStock;
        public string? ProfilePictureBase64 => profilePictureBase64; // <--- Getter
    }
}