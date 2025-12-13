using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.CreateCustomer;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Customers.GetCustomers;
using NorthWind.Sales.Entities.Dtos.Customers.CreateCustomer;
using NorthWind.Sales.Entities.Dtos.Customers.GetCustomers;

namespace Microsoft.AspNetCore.Builder;

public static class PerformanceCustomersController
{
    public static WebApplication UsePerformanceCustomersController(this WebApplication app)
    {
        const string ROLES_ADMIN = "SuperUser,Administrator";
        const string ROLES_LECTURA = "SuperUser,Administrator,Employee";

        app.MapPost("/api/performance/customers/insert", TestInsertCustomers)
            .WithName("TestInsertCustomers")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_ADMIN })
            .Produces<CustomersPerformanceResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/api/performance/customers/select", TestSelectCustomers)
            .WithName("TestSelectCustomers")
            .RequireAuthorization(new AuthorizeAttribute { Roles = ROLES_LECTURA })
            .Produces<CustomersPerformanceResultDto>(StatusCodes.Status200OK);

        return app;
    }

    #region DTOs
    public record CustomersPerformanceRequestDto(int Quantity);
    public record CustomersPerformanceResultDto(string Operation, int Quantity, long ElapsedMilliseconds, string Message);
    #endregion

    #region INSERT Masivo

    private static async Task<IResult> TestInsertCustomers(
        [FromBody] CustomersPerformanceRequestDto request,
        [FromServices] ICreateCustomerInputPort inputPort,
        [FromServices] ICreateCustomerOutputPort presenter)
    {
        if (request.Quantity <= 0 || request.Quantity > 100000)
            return Results.BadRequest(new { error = "La cantidad debe estar entre 1 y 100,000" });

        var stopwatch = Stopwatch.StartNew();
        int successCount = 0;
        var random = Random.Shared;

        Console.WriteLine($"\n[Performance] Iniciando inserción de {request.Quantity:N0} clientes...");

        try
        {
            for (int i = 1; i <= request.Quantity; i++)
            {
                // CAMBIO CLAVE: Generamos un ID de 10 caracteres.
                // Al ser de 10 chars, NUNCA chocará con los de 5 chars que ya tienes (00000-99999).
                string id = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

                string name = $"Perf_{id}_{i}"; // Nombre único
                string email = $"{id.ToLower()}@test.com";
                string cedula = random.NextInt64(1000000000, 9999999999).ToString();
                string password = "Pass123!";
                decimal balance = (decimal)(random.NextDouble() * 1000);

                var customerDto = new CreateCustomerDto(id, name, balance, email, cedula, password);

                try
                {
                    await inputPort.Handle(customerDto);
                    successCount++;

                    if (i % 100 == 0 || i == request.Quantity)
                        Console.WriteLine($"[Performance] Progreso: {successCount:N0}/{request.Quantity:N0} insertados");
                }
                catch (Exception ex)
                {
                    // Capturamos errores individuales para no detener todo el proceso
                    Console.WriteLine($"[Performance] ⚠ Error en fila {i}: {ex.Message.Split('\n')[0]}");
                }
            }

            stopwatch.Stop();
            var message = $"✓ Finalizado. Insertados: {successCount:N0}/{request.Quantity:N0}";
            Console.WriteLine($"[Performance] {message} en {stopwatch.ElapsedMilliseconds:N0} ms");

            return Results.Ok(new CustomersPerformanceResultDto("INSERT", successCount, stopwatch.ElapsedMilliseconds, message));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return Results.Ok(new CustomersPerformanceResultDto("INSERT", successCount, stopwatch.ElapsedMilliseconds, ex.Message));
        }
    }

    #endregion

    #region SELECT Masivo

    private static async Task<IResult> TestSelectCustomers(
        [FromBody] CustomersPerformanceRequestDto request,
        [FromServices] IGetCustomersInputPort inputPort,
        [FromServices] IGetCustomersOutputPort presenter)
    {
        if (request.Quantity <= 0) return Results.BadRequest(new { error = "Cantidad > 0" });

        var stopwatch = Stopwatch.StartNew();
        int totalFetched = 0;
        int currentPage = 1;
        const int PAGE_SIZE = 100;
        bool hasMoreData = true;

        Console.WriteLine($"\n[Performance] Consultando {request.Quantity:N0} clientes...");

        try
        {
            while (hasMoreData && totalFetched < request.Quantity)
            {
                int remaining = request.Quantity - totalFetched;
                int size = Math.Min(PAGE_SIZE, remaining);

                var queryDto = new GetCustomersQueryDto
                {
                    PageNumber = currentPage,
                    PageSize = size,
                    OrderDescending = false
                };

                await inputPort.Handle(queryDto);

                if (presenter.Result?.Customers != null && presenter.Result.Customers.Any())
                {
                    int count = presenter.Result.Customers.Count();
                    totalFetched += count;

                    // Solo imprimimos cada 10 páginas para no saturar la consola
                    if (currentPage % 10 == 0)
                        Console.WriteLine($"[Performance]   ✓ Pág {currentPage}: {count} rec. Total: {totalFetched:N0}");

                    // Validamos si se acabaron los datos
                    if (totalFetched >= request.Quantity || count < size)
                        hasMoreData = false;
                    else
                        currentPage++;
                }
                else
                {
                    hasMoreData = false;
                }

                // Pequeña pausa para no bloquear hilos del servidor
                if (hasMoreData) await Task.Delay(5);
            }

            stopwatch.Stop();
            string msg = $"✓ Se consultaron {totalFetched:N0} clientes en {stopwatch.ElapsedMilliseconds:N0} ms";
            Console.WriteLine($"[Performance] {msg}");

            return Results.Ok(new CustomersPerformanceResultDto("SELECT", totalFetched, stopwatch.ElapsedMilliseconds, msg));
        }
        catch (Exception ex)
        {
            return Results.Ok(new CustomersPerformanceResultDto("SELECT", totalFetched, stopwatch.ElapsedMilliseconds, ex.Message));
        }
    }
    #endregion
}