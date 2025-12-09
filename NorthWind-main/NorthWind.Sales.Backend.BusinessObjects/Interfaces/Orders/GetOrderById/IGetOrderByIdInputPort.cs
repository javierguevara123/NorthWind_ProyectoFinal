using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrderById
{
    public interface IGetOrderByIdInputPort
    {
        Task Handle(GetOrderByIdDto dto);
    }
}
