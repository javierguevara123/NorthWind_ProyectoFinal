namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.CreateCustomer
{
    public interface ICreateCustomerOutputPort
    {
        string CustomerId { get; }
        Task Handle(string customerId);
    }
}