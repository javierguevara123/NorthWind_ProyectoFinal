namespace NorthWind.Sales.Entities.Dtos.Products.CreateProduct
{
    public class CreateProductDto(
    string name,      
    short unitsInStock,  
    decimal unitPrice)   
    {
        public string Name => name;
        public short UnitsInStock => unitsInStock;
        public decimal UnitPrice => unitPrice;
    }
}
