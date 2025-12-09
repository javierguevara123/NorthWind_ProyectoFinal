namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.DeleteCustomer
{
    public interface IDeleteCustomerOutputPort
    {
        string CustomerId { get; }
        Task Handle(string customerId);

    }
}