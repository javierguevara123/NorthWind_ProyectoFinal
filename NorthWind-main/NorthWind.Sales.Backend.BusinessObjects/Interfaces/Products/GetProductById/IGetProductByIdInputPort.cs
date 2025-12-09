using NorthWind.Sales.Entities.Dtos.Products.GetProductById;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProductById
{
    public interface IGetProductByIdInputPort
    {
        Task Handle(GetProductByIdDto dto);
    }
}
