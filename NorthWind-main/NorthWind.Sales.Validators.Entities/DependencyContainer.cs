using Microsoft.Extensions.DependencyInjection;
using NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer;
using NorthWind.Sales.Entities.Dtos.Orders.CreateOrder;
using NorthWind.Sales.Entities.Dtos.Products.CreateProduct;
using NorthWind.Sales.Entities.Dtos.Products.UpdateProduct;
using NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer;
using NorthWind.Sales.Validators.Entities.Orders.CreateOrder;
using NorthWind.Sales.Validators.Entities.Products.CreateProduct;
using NorthWind.Sales.Validators.Entities.Products.UpdateProduct;
using NorthWind.Sales.Validators.Entities.Customers.CreateCustomer;
using NorthWind.Sales.Validators.Entities.Customers.UpdateCustomer;
using NorthWind.Validation.Entities;

namespace NorthWind.Sales.Validators.Entities;

public static class DependencyContainer
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddModelValidator<CreateOrderDto, CreateOrderDtoValidator>();
        services.AddModelValidator<CreateOrderDetailDto, CreateOrderDetailDtoValidator>();
        services.AddModelValidator<UpdateProductDto, UpdateProductDtoValidator>();
        services.AddModelValidator<CreateProductDto, CreateProductDtoValidator>();
        services.AddModelValidator<CreateCustomerDto, CreateCustomerDtoValidator>();
        services.AddModelValidator<UpdateCustomerDto, UpdateCustomerDtoValidator>();
        return services;
    }
}
