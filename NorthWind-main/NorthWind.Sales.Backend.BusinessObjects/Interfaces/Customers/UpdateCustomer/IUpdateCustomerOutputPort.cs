using NorthWind.Sales.Backend.BusinessObjects.Entities;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.UpdateCustomer
{
    public interface IUpdateCustomerOutputPort
    {
        string CustomerId { get; }
        Customer? Customer { get; }
        Task Handle(Customer customer);
    }

}