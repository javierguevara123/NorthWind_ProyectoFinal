namespace NorthWind.Sales.Entities.ValueObjects;

public class Endpoints
{
    // Orders
    public const string CreateOrder = $"/{nameof(CreateOrder)}";
    public const string GetOrderById = $"/{nameof(GetOrderById)}/{{id:int}}";
    public const string DeleteOrder = $"/{nameof(DeleteOrder)}/{{id:int}}";  // ← AGREGAR

    // Products
    public const string CreateProduct = $"/{nameof(CreateProduct)}";
    public const string UpdateProduct = $"/{nameof(UpdateProduct)}/{{id:int}}";
    public const string DeleteProduct = $"/{nameof(DeleteProduct)}/{{id:int}}";
    public const string GetProductById = $"/{nameof(GetProductById)}/{{id:int}}";

    // Customers
    public const string CreateCustomer = $"/{nameof(CreateCustomer)}";
    public const string UpdateCustomer = $"/{nameof(UpdateCustomer)}/{{id}}";
    public const string DeleteCustomer = $"/{nameof(DeleteCustomer)}/{{id}}";
    public const string GetCustomerById = $"/{nameof(GetCustomerById)}/{{id}}";
}