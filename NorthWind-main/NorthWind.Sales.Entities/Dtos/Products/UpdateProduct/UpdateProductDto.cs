namespace NorthWind.Sales.Entities.Dtos.Products.UpdateProduct
{
    public class UpdateProductDto(
        int productId,
        string name,
        short unitsInStock,
        decimal unitPrice,
        string? profilePictureBase64) // <--- NUEVO CAMPO
    {
        public int ProductId => productId;
        public string Name => name;
        public short UnitsInStock => unitsInStock;
        public decimal UnitPrice => unitPrice;
        public string? ProfilePictureBase64 => profilePictureBase64; // <--- Getter
    }
}