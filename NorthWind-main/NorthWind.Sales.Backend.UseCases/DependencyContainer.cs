
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.DeleteOrder;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrderById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrders;
using NorthWind.Sales.Backend.UseCases.Orders.DeleteOrder;
using NorthWind.Sales.Backend.UseCases.Orders.GetOrderById;
using NorthWind.Sales.Backend.UseCases.Orders.GetOrders;
using NorthWind.Sales.Entities.Dtos.Orders.DeleteOrder;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;

namespace NorthWind.Sales.Backend.UseCases;

public static class DependencyContainer
{
    public static IServiceCollection AddUseCasesServices(this IServiceCollection services)
    {
        services.AddScoped<ICreateOrderInputPort, CreateOrderInteractor>();
        services.AddScoped<ICreateProductInputPort, CreateProductInteractor>();
        services.AddScoped<IUpdateProductInputPort, UpdateProductInteractor>();
        services.AddScoped<IDeleteProductInputPort, DeleteProductInteractor>();
        services.AddScoped<IGetProductByIdInputPort, GetProductByIdInteractor>();
        services.AddScoped<IGetProductsInputPort, GetProductsInteractor>();

        //CUSTOMERS
        services.AddScoped<ICreateCustomerInputPort, CreateCustomerInteractor>();
        services.AddScoped<IUpdateCustomerInputPort, UpdateCustomerInteractor>();
        services.AddScoped<IDeleteCustomerInputPort, DeleteCustomerInteractor>();
        services.AddScoped<IGetCustomerByIdInputPort, GetCustomerByIdInteractor>();
        services.AddScoped<IGetCustomersInputPort, GetCustomersInteractor>();

        services.AddScoped<IGetOrderByIdInputPort, GetOrderByIdInteractor>();
        services.AddScoped<IGetOrdersInputPort, GetOrdersInteractor>();
        services.AddScoped<IDeleteOrderInputPort, DeleteOrderInteractor>();


        services.AddModelValidator<CreateOrderDto, CreateOrderCustomerValidator>();
        services.AddModelValidator<CreateOrderDto, CreateOrderProductValidator>();
        services.AddModelValidator<CreateProductDto, CreateProductBusinessValidator>();
        services.AddModelValidator<UpdateProductDto, UpdateProductBusinessValidator>();
        services.AddModelValidator<DeleteProductDto, DeleteProductBusinessValidator>();
        services.AddModelValidator<GetProductByIdDto, GetProductByIdValidator>();

        services.AddModelValidator<GetOrderByIdDto, GetOrderByIdValidator>();
        services.AddModelValidator<GetOrdersQueryDto, GetOrdersValidator>();
        services.AddModelValidator<DeleteOrderDto, DeleteOrderBusinessValidator>();

        //CUSTOMERS

        services.AddModelValidator<CreateCustomerDto, CreateCustomerBusinessValidator>();
        services.AddModelValidator<UpdateCustomerDto, UpdateCustomerBusinessValidator>();
        services.AddModelValidator<DeleteCustomerDto, DeleteCustomerBusinessValidator>();
        services.AddModelValidator<GetCustomerByIdDto, GetCustomerByIdValidator>();


        services.AddScoped<IDomainEventHandler<SpecialOrderCreatedEvent>, SendEMailWhenSpecialOrderCreatedEventHandler>();

        return services;
    }

}