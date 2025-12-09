using NorthWind.Sales.Entities.Dtos.Orders.DeleteOrder;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.DeleteOrder
{
    public interface IDeleteOrderInputPort
    {
        Task Handle(DeleteOrderDto dto);
    }
}
