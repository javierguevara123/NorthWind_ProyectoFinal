using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.CreateProduct;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.DeleteProduct;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProductById;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProducts;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.UpdateProduct;
using NorthWind.Sales.Entities.Dtos.Products.CreateProduct;
using NorthWind.Sales.Entities.Dtos.Products.DeleteProduct;
using NorthWind.Sales.Entities.Dtos.Products.GetProductById;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;
using NorthWind.Sales.Entities.Dtos.Products.UpdateProduct;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Controller consolidado para todos los endpoints de productos.
/// </summary>
public static class ProductsController
{
    public static WebApplication UseProductsController(this WebApplication app)
    {
        // Definimos los grupos de roles para reutilizar y evitar errores de escritura
        const string ROLES_LECTURA = "SuperUser,Administrator,Employee";
        const string ROLES_ESCRITURA = "SuperUser,Administrator";

        // GET: Obtener lista (Permitido para todos: SuperUser, Admin y Employee)
        app.MapGet("/api/products", GetProducts)
            .WithName("GetProducts")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_LECTURA })
            .Produces<PagedResultDto<ProductDto>>(StatusCodes.Status200OK);

        // GET: Obtener por ID (Permitido para todos)
        app.MapGet(Endpoints.GetProductById, GetProductById)
            .WithName("GetProductById")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_LECTURA })
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // POST: Crear (Solo SuperUser y Administrator)
        app.MapPost(Endpoints.CreateProduct, CreateProduct)
            .WithName("CreateProduct")
            .Produces<int>(StatusCodes.Status201Created)
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ESCRITURA });

        // PUT: Actualizar (Solo SuperUser y Administrator)
        app.MapPut(Endpoints.UpdateProduct, UpdateProduct)
            .WithName("UpdateProduct")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ESCRITURA })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        // DELETE: Eliminar (Solo SuperUser y Administrator)
        app.MapDelete(Endpoints.DeleteProduct, DeleteProduct)
            .WithName("DeleteProduct")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ESCRITURA })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    #region GET Endpoints

    private static async Task<IResult> GetProducts(
        [AsParameters] GetProductsQueryDto query,
        IGetProductsInputPort inputPort,
        IGetProductsOutputPort presenter)
    {
        await inputPort.Handle(query);
        return Results.Ok(presenter.Result);
    }

    private static async Task<IResult> GetProductById(
        int id,
        IGetProductByIdInputPort inputPort,
        IGetProductByIdOutputPort presenter)
    {
        var dto = new GetProductByIdDto(id);
        await inputPort.Handle(dto);

        if (presenter.Product == null)
        {
            return Results.NotFound(new
            {
                error = $"Producto con Id {id} no encontrado"
            });
        }

        return Results.Ok(presenter.Product);
    }

    #endregion

    #region POST Endpoints

    private static async Task<IResult> CreateProduct(
        CreateProductDto productDto,
        ICreateProductInputPort inputPort,
        ICreateProductOutputPort presenter)
    {
        await inputPort.Handle(productDto);

        return Results.Created(
            $"/api/products/{presenter.ProductId}",
            new
            {
                id = presenter.ProductId,
                message = "Producto creado exitosamente"
            });
    }

    #endregion

    #region PUT Endpoints

    private static async Task<IResult> UpdateProduct(
        int id,
        UpdateProductDto dto,
        IUpdateProductInputPort inputPort,
        IUpdateProductOutputPort presenter)
    {
        if (id != dto.ProductId)
        {
            return Results.BadRequest(new
            {
                error = "El Id de la URL no coincide con el Id del producto"
            });
        }

        await inputPort.Handle(dto);

        return Results.Ok(new
        {
            id = presenter.ProductId,
            message = "Producto actualizado exitosamente"
        });
    }

    #endregion

    #region DELETE Endpoints

    private static async Task<IResult> DeleteProduct(
        int id,
        IDeleteProductInputPort inputPort,
        IDeleteProductOutputPort presenter)
    {
        var dto = new DeleteProductDto(id);
        await inputPort.Handle(dto);

        return Results.Ok(new
        {
            id = presenter.ProductId,
            message = "Producto eliminado exitosamente"
        });
    }

    #endregion
}