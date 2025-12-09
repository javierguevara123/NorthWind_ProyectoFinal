using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.CreateCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.DeleteCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomerById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomers;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.UpdateCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.DeleteOrder;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrderById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrders;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.CreateProduct;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.DeleteProduct;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProductById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProducts;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.UpdateProduct;
using NorthWind.Sales.Backend.Presenters.Customers.CreateCustomer;
using NorthWind.Sales.Backend.Presenters.Customers.DeleteCustomer;
using NorthWind.Sales.Backend.Presenters.Customers.GetCustomers;
using NorthWind.Sales.Backend.Presenters.Customers.UpdateCustomer;
using NorthWind.Sales.Backend.Presenters.Orders.DeleteOrder;
using NorthWind.Sales.Backend.Presenters.Orders.GetOrderById;
using NorthWind.Sales.Backend.Presenters.Orders.GetOrders;
using NorthWind.Sales.Backend.Presenters.Products.CreateProduct;
using NorthWind.Sales.Backend.Presenters.Products.DeleteProduct;
using NorthWind.Sales.Backend.Presenters.Products.GetProductById;
using NorthWind.Sales.Backend.Presenters.Products.GetProducts;
using NorthWind.Sales.Backend.Presenters.Products.UpdateProduct;
using NorthWind.Sales.Backend.PresentersCustomer.GetCustomerById;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyContainer
{
    public static IServiceCollection AddPresenters(this IServiceCollection services)
    {
        services.AddScoped<ICreateOrderOutputPort, CreateOrderPresenter>();
        services.AddScoped<ICreateProductOutputPort, CreateProductPresenter>();
        services.AddScoped<IUpdateProductOutputPort, UpdateProductPresenter>();
        services.AddScoped<IDeleteProductOutputPort, DeleteProductPresenter>();
        services.AddScoped<IGetProductByIdOutputPort, GetProductByIdPresenter>();
        services.AddScoped<IGetProductsOutputPort, GetProductsPresenter>();

        services.AddScoped<ICreateCustomerOutputPort, CreateCustomerPresenter>();
        services.AddScoped<IUpdateCustomerOutputPort, UpdateCustomerPresenter>();
        services.AddScoped<IDeleteCustomerOutputPort, DeleteCustomerPresenter>();
        services.AddScoped<IGetCustomerByIdOutputPort, GetCustomerByIdPresenter>();
        services.AddScoped<IGetCustomersOutputPort, GetCustomersPresenter>();

        services.AddScoped<IGetOrderByIdOutputPort, GetOrderByIdPresenter>();
        services.AddScoped<IGetOrdersOutputPort, GetOrdersPresenter>();
        services.AddScoped<IDeleteOrderOutputPort, DeleteOrderPresenter>();

        return services;
    }
}

