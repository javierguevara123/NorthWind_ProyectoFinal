using NorthWind.Sales.Entities.Dtos.Products.GetProductById;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProductById
{
    public interface IGetProductByIdOutputPort
    {
        ProductDetailDto? Product { get; }
        Task Handle(ProductDetailDto? product);
    }
}
