using System.Security.Claims; // <--- NECESARIO
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrderById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.GetOrders;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Orders.DeleteOrder;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;
using NorthWind.Sales.Entities.Dtos.Orders.DeleteOrder;

namespace Microsoft.AspNetCore.Builder;

public static class OrdersController
{
    public static WebApplication UseOrdersController(this WebApplication app)
    {
        // ✅ CAMBIO: Roles permitidos
        const string ROLES_ALL = "SuperUser,Administrator,Employee,Customer";
        const string ROLES_ADMIN = "SuperUser,Administrator";

        // POST: Crear nueva orden (Permitido para todos)
        app.MapPost(Endpoints.CreateOrder, CreateOrder)
            .WithName("CreateOrder")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ALL })
            .Produces<int>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden);

        // GET: Obtener lista paginada de órdenes (Permitido para todos)
        app.MapGet("/api/orders", GetOrders)
            .WithName("GetOrders")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ALL })
            .Produces<OrderPagedResultDto>(StatusCodes.Status200OK);

        // GET: Obtener orden por ID con detalles (Permitido para todos)
        app.MapGet(Endpoints.GetOrderById, GetOrderById)
            .WithName("GetOrderById")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ALL })
            .Produces<OrderWithDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // DELETE: Eliminar orden (SOLO ADMIN)
        app.MapDelete(Endpoints.DeleteOrder, DeleteOrder)
            .WithName("DeleteOrder")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ADMIN })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    #region POST Endpoints

    private static async Task<IResult> CreateOrder(
        CreateOrderDto orderDto,
        HttpContext httpContext, // <--- Inyección del Contexto
        ICreateOrderInputPort inputPort,
        ICreateOrderOutputPort presenter)
    {
        // ✅ LÓGICA DE SEGURIDAD PARA CLIENTES
        var user = httpContext.User;
        if (user.IsInRole("Customer"))
        {
            // El .Trim() aquí actúa como un "seguro" por si el token viene sucio
            var tokenCustomerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value?.Trim();

            if (!string.Equals(orderDto.CustomerId, tokenCustomerId, StringComparison.OrdinalIgnoreCase))
            {
                return Results.Json(new { error = "No autorizado..." }, statusCode: 403);
            }
        }

        await inputPort.Handle(orderDto);

        return Results.Created(
            $"/api/orders/{presenter.OrderId}",
            new
            {
                id = presenter.OrderId,
                message = "Orden creada exitosamente"
            });
    }

    #endregion

    #region GET Endpoints

    private static async Task<IResult> GetOrders(
        [AsParameters] GetOrdersQueryDto query,
        HttpContext httpContext, // <--- Inyección del Contexto
        IGetOrdersInputPort inputPort,
        IGetOrdersOutputPort presenter)
    {
        // ✅ LÓGICA DE SEGURIDAD PARA CLIENTES
        var user = httpContext.User;
        if (user.IsInRole("Customer"))
        {
            // Forzamos el filtro para que solo vea sus órdenes
            query.CustomerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        await inputPort.Handle(query);
        return Results.Ok(presenter.Result);
    }

    private static async Task<IResult> GetOrderById(
        int id,
        HttpContext httpContext, // <--- Inyección del Contexto
        IGetOrderByIdInputPort inputPort,
        IGetOrderByIdOutputPort presenter)
    {
        var dto = new GetOrderByIdDto(id);
        await inputPort.Handle(dto);

        if (presenter.Order == null)
        {
            return Results.NotFound(new
            {
                error = $"Orden con Id {id} no encontrada"
            });
        }

        // ✅ LÓGICA DE SEGURIDAD PARA CLIENTES
        var user = httpContext.User;
        if (user.IsInRole("Customer"))
        {
            var tokenCustomerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Si la orden no es suya, retornamos NotFound para ocultarla
            if (!string.Equals(presenter.Order.CustomerId, tokenCustomerId, StringComparison.OrdinalIgnoreCase))
            {
                return Results.NotFound(new { error = $"Orden con Id {id} no encontrada" });
            }
        }

        return Results.Ok(presenter.Order);
    }

    #endregion

    #region DELETE Endpoints

    private static async Task<IResult> DeleteOrder(
        int id,
        IDeleteOrderInputPort inputPort,
        IDeleteOrderOutputPort presenter)
    {
        var dto = new DeleteOrderDto(id);
        await inputPort.Handle(dto);

        return Results.Ok(new
        {
            id = presenter.OrderId,
            message = "Orden eliminada exitosamente"
        });
    }

    #endregion
}