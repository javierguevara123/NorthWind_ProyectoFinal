
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.CreateCustomer;


namespace NorthWind.Sales.Backend.Presenters.Customers.CreateCustomer
{
    internal class CreateCustomerPresenter : ICreateCustomerOutputPort
    {
        public string CustomerId { get; private set; }
        public Task Handle(string customerId)
        {
            CustomerId = customerId;
            return Task.CompletedTask;
        }
    }
}
