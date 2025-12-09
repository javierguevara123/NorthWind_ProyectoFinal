using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.DeleteOrder;

namespace NorthWind.Sales.Backend.Presenters.Orders.DeleteOrder
{
    internal class DeleteOrderPresenter : IDeleteOrderOutputPort
    {
        public int OrderId { get; private set; }

        public Task Handle(int orderId)
        {
            OrderId = orderId;
            return Task.CompletedTask;
        }
    }
}
