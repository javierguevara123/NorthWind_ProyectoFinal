using NorthWind.Sales.Entities.Dtos.Products.DeleteProduct;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.DeleteProduct
{
    public interface IDeleteProductInputPort
    {
        Task Handle(DeleteProductDto dto);
    }
}
