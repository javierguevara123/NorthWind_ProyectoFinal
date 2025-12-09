
using Microsoft.AspNetCore.Http;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.CreateCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.DeleteCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomerById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomers;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.UpdateCustomer;
using NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer;
using NorthWind.Sales.Entities.Dtos.Customers.DeleteCustomer;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;
using NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer;

namespace Microsoft.AspNetCore.Builder;

public static class CustomersController
{
    public static WebApplication UseCustomersController(this WebApplication app)
    {
        // CREATE
        app.MapPost(Endpoints.CreateCustomer, CreateCustomer)
           .RequireAuthorization()
           .Produces<string>(StatusCodes.Status200OK);

        // DELETE
        app.MapDelete(Endpoints.DeleteCustomer, DeleteCustomer)
           .RequireAuthorization()
           .WithName("DeleteCustomer");

        // GET ALL (paginado)
        app.MapGet("/api/customers", GetCustomers)
           .WithName("GetCustomers")
           .Produces<CustomerPagedResultDto>(StatusCodes.Status200OK);

        // GET BY ID
        app.MapGet(Endpoints.GetCustomerById, GetCustomerById)
           .RequireAuthorization();

        // UPDATE
        app.MapPut(Endpoints.UpdateCustomer, UpdateCustomer)
           .RequireAuthorization();

        return app;
    }

    #region Handlers
    public static async Task<IResult> CreateCustomer(
    CreateCustomerDto customerDto,
    ICreateCustomerInputPort inputPort,
    ICreateCustomerOutputPort presenter)
    {
        await inputPort.Handle(customerDto);
        return Results.Ok(new { id = presenter.CustomerId });   // ← objeto anónimo
    }

    private static async Task<IResult> GetCustomers(
        [AsParameters] GetCustomersQueryDto query,
        IGetCustomersInputPort inputPort,
        IGetCustomersOutputPort presenter)
    {
        await inputPort.Handle(query);
        return Results.Ok(presenter.Result);
    }

    private static async Task DeleteCustomer(
    string id,  // ← Cambiar de "customerId" a "id"
    IDeleteCustomerInputPort inputPort,
    IDeleteCustomerOutputPort presenter)
    {
        var dto = new DeleteCustomerDto(id);
        await inputPort.Handle(dto);
        _ = presenter.CustomerId;
    }

    private static async Task<IResult> GetCustomerById(
        string id,  // ← Cambiar de "customerId" a "id"
        IGetCustomerByIdInputPort inputPort,
        IGetCustomerByIdOutputPort presenter)
    {
        var dto = new GetCustomerByIdDto(id);
        await inputPort.Handle(dto);

        return presenter.Customer is null
            ? Results.NotFound(new { error = $"Cliente con Id {id} no encontrado" })
            : Results.Ok(presenter.Customer);
    }

    private static async Task<IResult> UpdateCustomer(
        string id,  // ← Ya está correcto
        UpdateCustomerDto dto,
        IUpdateCustomerInputPort inputPort,
        IUpdateCustomerOutputPort presenter)
    {
        if (id != dto.CustomerId)
            return Results.BadRequest(new { error = "El Id de la URL no coincide con el Id del cliente" });

        await inputPort.Handle(dto);
        return Results.Ok(new
        {
            id = presenter.CustomerId,
            message = "Cliente actualizado exitosamente"
        });
    }
    #endregion
}