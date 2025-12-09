namespace NorthWind.Sales.Entities.Dtos.Products.UpdateProduct
{
    public class UpdateProductDto(
    int productId, 
    string name,       
    short unitsInStock,
    decimal unitPrice)
    {
        public int ProductId => productId;
        public string Name => name;
        public short UnitsInStock => unitsInStock;
        public decimal UnitPrice => unitPrice;
    }
}
