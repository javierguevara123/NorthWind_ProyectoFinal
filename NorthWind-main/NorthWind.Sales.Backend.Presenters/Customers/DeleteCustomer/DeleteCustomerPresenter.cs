using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.DeleteCustomer;

namespace NorthWind.Sales.Backend.Presenters.Customers.DeleteCustomer
{
    
    internal class DeleteCustomerPresenter : IDeleteCustomerOutputPort
    {
        public string CustomerId { get; private set; }
        public Task Handle(string customerId)
        {
            CustomerId = customerId;
            return Task.CompletedTask;
        }
    }
}