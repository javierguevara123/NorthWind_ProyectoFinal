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
        // POST: Crear nueva orden
        app.MapPost(Endpoints.CreateOrder, CreateOrder)
            .WithName("CreateOrder")
            .RequireAuthorization()
            .Produces<int>(StatusCodes.Status201Created);

        // GET: Obtener lista paginada de órdenes
        app.MapGet("/api/orders", GetOrders)
            .WithName("GetOrders")
            .RequireAuthorization()
            .Produces<OrderPagedResultDto>(StatusCodes.Status200OK);

        // GET: Obtener orden por ID con detalles
        app.MapGet(Endpoints.GetOrderById, GetOrderById)
            .WithName("GetOrderById")
            .RequireAuthorization()
            .Produces<OrderWithDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // DELETE: Eliminar orden
        app.MapDelete(Endpoints.DeleteOrder, DeleteOrder)
            .WithName("DeleteOrder")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    #region POST Endpoints

    private static async Task<IResult> CreateOrder(
        CreateOrderDto orderDto,
        ICreateOrderInputPort inputPort,
        ICreateOrderOutputPort presenter)
    {
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
        IGetOrdersInputPort inputPort,
        IGetOrdersOutputPort presenter)
    {
        await inputPort.Handle(query);
        return Results.Ok(presenter.Result);
    }

    private static async Task<IResult> GetOrderById(
        int id,
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