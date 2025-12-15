using Microsoft.AspNetCore.Authorization; // 👈 Importante para AuthorizeAttribute
using Microsoft.AspNetCore.Http;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.CreateCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.DeleteCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomerById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomers;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.Login;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.UpdateCustomer;
using NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer;
using NorthWind.Sales.Entities.Dtos.Customers.DeleteCustomer;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomerById;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;
using NorthWind.Sales.Entities.Dtos.Customers.Login;
using NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer;

namespace Microsoft.AspNetCore.Builder;

public static class CustomersController
{
    public static WebApplication UseCustomersController(this WebApplication app)
    {
        // 🛡️ DEFINICIÓN DE ROLES
        const string ROLES_WRITER = "SuperUser,Administrator";
        const string ROLES_READER = "SuperUser,Administrator,Employee";

        // ==========================================
        // ✅ ENDPOINTS PÚBLICOS (Login & Register)
        // ==========================================

        // REGISTER
        app.MapPost("/api/customers/register", RegisterCustomer)
           .AllowAnonymous()
           .Produces<string>(StatusCodes.Status200OK);

        // LOGIN
        app.MapPost("/api/customers/login", LoginCustomer)
           .AllowAnonymous()
           .Produces<LoginResponseDto>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);


        // ==========================================
        // 🔒 ENDPOINTS PROTEGIDOS (Roles)
        // ==========================================

        // CREATE (Solo Escritura)
        app.MapPost(Endpoints.CreateCustomer, CreateCustomer)
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_WRITER })
            .Produces<string>(StatusCodes.Status200OK);

        // DELETE (Solo Escritura)
        app.MapDelete(Endpoints.DeleteCustomer, DeleteCustomer)
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_WRITER })
            .WithName("DeleteCustomer");

        // GET ALL (Lectura)
        app.MapGet("/api/customers", GetCustomers)
            .WithName("GetCustomers")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_READER })
            .Produces<CustomerPagedResultDto>(StatusCodes.Status200OK);

        // GET BY ID (Lectura)
        app.MapGet(Endpoints.GetCustomerById, GetCustomerById)
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_READER });

        // UPDATE (Solo Escritura)
        app.MapPut(Endpoints.UpdateCustomer, UpdateCustomer)
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_WRITER });

        return app;
    }

    #region Handlers

    // Handler de Registro
    public static async Task<IResult> RegisterCustomer(
        CreateCustomerDto customerDto,
        ICreateCustomerInputPort inputPort,
        ICreateCustomerOutputPort presenter)
    {
        await inputPort.Handle(customerDto);
        return Results.Ok(new
        {
            message = "Cliente registrado exitosamente",
            id = presenter.CustomerId
        });
    }

    // Handler de Login
    public static async Task<IResult> LoginCustomer(
        LoginCustomerDto loginDto,
        ILoginCustomerInputPort inputPort)
    {
        try
        {
            var response = await inputPort.Handle(loginDto);
            return Results.Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error interno: {ex.Message}");
        }
    }

    // Handler Create (Admin)
    public static async Task<IResult> CreateCustomer(
        CreateCustomerDto customerDto,
        ICreateCustomerInputPort inputPort,
        ICreateCustomerOutputPort presenter)
    {
        await inputPort.Handle(customerDto);
        return Results.Ok(new { id = presenter.CustomerId });
    }

    // Handler Get All
    private static async Task<IResult> GetCustomers(
        [AsParameters] GetCustomersQueryDto query,
        IGetCustomersInputPort inputPort,
        IGetCustomersOutputPort presenter)
    {
        await inputPort.Handle(query);
        return Results.Ok(presenter.Result);
    }

    // Handler Delete
    private static async Task DeleteCustomer(
        string id,
        IDeleteCustomerInputPort inputPort,
        IDeleteCustomerOutputPort presenter)
    {
        var dto = new DeleteCustomerDto(id);
        await inputPort.Handle(dto);
        _ = presenter.CustomerId;
    }

    // Handler Get By ID
    private static async Task<IResult> GetCustomerById(
        string id,
        IGetCustomerByIdInputPort inputPort,
        IGetCustomerByIdOutputPort presenter)
    {
        var dto = new GetCustomerByIdDto(id);
        await inputPort.Handle(dto);

        return presenter.Customer is null
            ? Results.NotFound(new { error = $"Cliente con Id {id} no encontrado" })
            : Results.Ok(presenter.Customer);
    }

    // Handler Update
    private static async Task<IResult> UpdateCustomer(
        string id,
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