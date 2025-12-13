using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.CreateProduct;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Products.GetProducts;
using NorthWind.Sales.Entities.Dtos.Products.CreateProduct;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Controller para pruebas de rendimiento (Performance Testing)
/// </summary>
public static class PerformanceProductsController
{
    public static WebApplication UsePerformanceProductsController(this WebApplication app)
    {
        const string ROLES_ADMIN = "SuperUser,Administrator";
        const string ROLES_LECTURA = "SuperUser,Administrator,Employee";

        // POST: Inserción masiva de productos
        app.MapPost("/api/performance/products/insert", TestInsertProducts)
            .WithName("TestInsertProducts")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ADMIN })
            .Produces<ProductsPerformanceResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        // POST: Consulta masiva de productos con paginación automática
        app.MapPost("/api/performance/products/select", TestSelectProducts)
            .WithName("TestSelectProducts")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_LECTURA })
            .Produces<ProductsPerformanceResultDto>(StatusCodes.Status200OK);

        return app;
    }

    #region DTOs

    public record ProductsPerformanceRequestDto(int Quantity);

    public record ProductsPerformanceResultDto(
        string Operation,
        int Quantity,
        long ElapsedMilliseconds,
        string Message
    );

    #endregion

    #region INSERT Masivo

    /// <summary>
    /// Inserta productos masivamente para pruebas de rendimiento
    /// </summary>
    private static async Task<IResult> TestInsertProducts(
        [FromBody] ProductsPerformanceRequestDto request,
        [FromServices] ICreateProductInputPort inputPort,
        [FromServices] ICreateProductOutputPort presenter)
    {
        // Validación
        if (request.Quantity <= 0 || request.Quantity > 100000)
        {
            return Results.BadRequest(new
            {
                error = "La cantidad debe estar entre 1 y 100,000"
            });
        }

        var stopwatch = Stopwatch.StartNew();
        int successCount = 0;
        var random = Random.Shared;

        Console.WriteLine($"\n[Performance] Iniciando inserción de {request.Quantity:N0} productos...");

        try
        {
            for (int i = 1; i <= request.Quantity; i++)
            {
                // Generar datos aleatorios
                string name = $"PerfTest_{Guid.NewGuid().ToString()[..8]}";
                short stock = (short)random.Next(1, 500);
                decimal price = (decimal)(random.NextDouble() * 1000);

                // Crear DTO (ImageUrl se maneja en el Repository)
                var productDto = new CreateProductDto(name, stock, price);

                try
                {
                    // Ejecutar caso de uso
                    await inputPort.Handle(productDto);

                    // CORRECCIÓN: Asumir éxito si no hay excepción
                    // El problema es que presenter.ProductId podría no actualizarse correctamente
                    // pero si llegamos aquí sin excepción, el INSERT fue exitoso
                    successCount++;

                    // Mostrar progreso cada 100 registros para pruebas pequeñas
                    if (i % 100 == 0 || i == request.Quantity)
                    {
                        Console.WriteLine($"[Performance] Progreso: {successCount:N0}/{request.Quantity:N0} productos insertados exitosamente");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Performance] ✗ Error al insertar producto {i}: {ex.Message}");
                    // Continuar con el siguiente producto
                }
            }

            stopwatch.Stop();

            var message = $"✓ Se insertaron {successCount:N0} productos exitosamente";
            Console.WriteLine($"[Performance] {message} en {stopwatch.ElapsedMilliseconds:N0} ms");

            return Results.Ok(new ProductsPerformanceResultDto(
                "INSERT",
                successCount,
                stopwatch.ElapsedMilliseconds,
                message
            ));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var errorMsg = $"✗ Error después de {successCount:N0} inserciones: {ex.Message}";
            Console.WriteLine($"[Performance] {errorMsg}");

            return Results.Ok(new ProductsPerformanceResultDto(
                "INSERT",
                successCount,
                stopwatch.ElapsedMilliseconds,
                errorMsg
            ));
        }
    }

    #endregion

    #region SELECT Masivo con Paginación

    /// <summary>
    /// Consulta productos masivamente usando paginación automática
    /// NOTA: Este endpoint maneja la paginación internamente para obtener 
    /// la cantidad exacta de productos solicitada
    /// </summary>
    private static async Task<IResult> TestSelectProducts(
        [FromBody] ProductsPerformanceRequestDto request,
        [FromServices] IGetProductsInputPort inputPort,
        [FromServices] IGetProductsOutputPort presenter)
    {
        // Validación
        if (request.Quantity <= 0)
        {
            return Results.BadRequest(new
            {
                error = "La cantidad debe ser mayor a 0"
            });
        }

        var stopwatch = Stopwatch.StartNew();
        int totalFetched = 0;
        int currentPage = 1;
        const int PAGE_SIZE = 100; // Respetar el límite de tu validación
        bool hasMoreData = true;
        int pagesProcessed = 0;

        Console.WriteLine($"\n[Performance] Iniciando consulta de {request.Quantity:N0} productos...");
        Console.WriteLine($"[Performance] Estrategia: Páginas de {PAGE_SIZE} productos hasta alcanzar objetivo");

        try
        {
            while (hasMoreData && totalFetched < request.Quantity)
            {
                // Calcular cuántos productos necesitamos en esta página
                int remainingNeeded = request.Quantity - totalFetched;
                int pageSizeToRequest = Math.Min(PAGE_SIZE, remainingNeeded);

                Console.WriteLine($"[Performance] → Solicitando página {currentPage} (tamaño: {pageSizeToRequest})...");

                // Crear query con paginación
                var queryDto = new GetProductsQueryDto
                {
                    PageNumber = currentPage,
                    PageSize = pageSizeToRequest,
                    OrderBy = "name",
                    OrderDescending = false
                };

                // Ejecutar caso de uso
                await inputPort.Handle(queryDto);

                // Verificar si hay resultados
                if (presenter.Result?.Items != null && presenter.Result.Items.Any())
                {
                    int fetchedInPage = presenter.Result.Items.Count();
                    totalFetched += fetchedInPage;
                    pagesProcessed++;

                    // Debug: Mostrar info del presenter
                    Console.WriteLine($"[Performance]   ✓ Recibidos: {fetchedInPage} productos");
                    Console.WriteLine($"[Performance]   📊 Total acumulado: {totalFetched:N0}/{request.Quantity:N0}");
                    Console.WriteLine($"[Performance]   🔍 HasNextPage: {presenter.Result.HasNextPage}");
                    Console.WriteLine($"[Performance]   📄 TotalCount: {presenter.Result.TotalCount}");
                    Console.WriteLine($"[Performance]   📑 TotalPages: {presenter.Result.TotalPages}");

                    // Verificar si ya tenemos suficientes productos
                    if (totalFetched >= request.Quantity)
                    {
                        hasMoreData = false;
                        Console.WriteLine($"[Performance] ✓ OBJETIVO ALCANZADO: {totalFetched:N0} productos obtenidos");
                    }
                    // Verificar si hay más páginas disponibles
                    else if (!presenter.Result.HasNextPage)
                    {
                        hasMoreData = false;
                        Console.WriteLine($"[Performance] ⚠ NO HAY MÁS DATOS EN BD. Total disponible: {totalFetched:N0} de {presenter.Result.TotalCount}");
                    }
                    else
                    {
                        currentPage++;
                        Console.WriteLine($"[Performance]   ⏭ Siguiente página: {currentPage}");
                    }
                }
                else
                {
                    // No hay más datos
                    hasMoreData = false;
                    Console.WriteLine($"[Performance] ⚠ PÁGINA VACÍA. Fin de datos en BD.");
                }

                Console.WriteLine(); // Línea en blanco para separar páginas

                // Pequeña pausa para no saturar el servidor (opcional)
                if (hasMoreData)
                {
                    await Task.Delay(10);
                }
            }

            stopwatch.Stop();

            // Determinar mensaje según resultado
            string message;
            if (totalFetched >= request.Quantity)
            {
                message = $"✓ Se consultaron {totalFetched:N0} productos exitosamente en {pagesProcessed} páginas";
            }
            else
            {
                message = $"⚠ Se consultaron {totalFetched:N0} de {request.Quantity:N0} solicitados (Total disponible en BD) en {pagesProcessed} páginas";
            }

            Console.WriteLine($"[Performance] {message} - Tiempo: {stopwatch.ElapsedMilliseconds:N0} ms");

            return Results.Ok(new ProductsPerformanceResultDto(
                "SELECT",
                totalFetched,
                stopwatch.ElapsedMilliseconds,
                message
            ));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var errorMsg = $"✗ Error después de consultar {totalFetched:N0} productos: {ex.Message}";
            Console.WriteLine($"[Performance] {errorMsg}");

            return Results.Ok(new ProductsPerformanceResultDto(
                "SELECT",
                totalFetched,
                stopwatch.ElapsedMilliseconds,
                errorMsg
            ));
        }
    }

    #endregion
}