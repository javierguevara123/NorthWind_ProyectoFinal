using NorthWind.Sales.Backend.BusinessObjects.Entities;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.UpdateCustomer;

namespace NorthWind.Sales.Backend.Presenters.Customers.UpdateCustomer
{
    
    /// <summary>
    /// Presenter para el caso de uso "Actualizar Cliente".
    /// </summary>
    internal class UpdateCustomerPresenter : IUpdateCustomerOutputPort
    {
        public string CustomerId { get; private set; }
        public Customer? Customer { get; private set; }
        public Task Handle(Customer customer)
        {
            CustomerId = customer.Id;
            Customer = customer;
            return Task.CompletedTask;
        }
    }
}
