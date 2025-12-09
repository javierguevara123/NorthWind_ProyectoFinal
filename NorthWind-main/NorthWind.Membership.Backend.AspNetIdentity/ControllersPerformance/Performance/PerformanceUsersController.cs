using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// 1. Agregar el namespace de tus entidades y contexto
using NorthWind.Membership.Backend.AspNetIdentity.Entities;
using NorthWind.Membership.Backend.AspNetIdentity.DataContexts;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Controller para pruebas de rendimiento de usuarios
/// OMITE TODAS LAS VALIDACIONES para máxima velocidad
/// </summary>
internal static class PerformanceUsersController
{
    public static WebApplication UsePerformanceUsersController(this WebApplication app)
    {
        const string ROLES_ADMIN = "SuperUser,Administrator";

        // POST: Inserción masiva de usuarios (SIN VALIDACIONES)
        app.MapPost("/api/performance/users/insert",
            [Authorize(Roles = "SuperUser,Administrator")]
        // 2. Cambiar IdentityUser por NorthWindUser y DbContext por NorthWindMembershipContext
        async ([FromBody] UsersPerformanceRequestDto request,
                   [FromServices] UserManager<NorthWindUser> userManager,
                   [FromServices] NorthWindMembershipContext dbContext) =>
            {
                return await TestInsertUsers(request, userManager, dbContext);
            })
            .WithName("TestInsertUsers")
            .RequireAuthorization()
            .Produces<UsersPerformanceResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        // POST: Consulta masiva de usuarios con paginación
        app.MapPost("/api/performance/users/select",
            [Authorize(Roles = "SuperUser,Administrator")]
        // 3. Cambiar IdentityUser por NorthWindUser
        async ([FromBody] UsersPerformanceRequestDto request,
                   [FromServices] UserManager<NorthWindUser> userManager) =>
            {
                return await TestSelectUsers(request, userManager);
            })
            .WithName("TestSelectUsers")
            .RequireAuthorization()
            .Produces<UsersPerformanceResultDto>(StatusCodes.Status200OK);

        return app;
    }

    #region DTOs

    public record UsersPerformanceRequestDto(int Quantity);

    public record UsersPerformanceResultDto(
        string Operation,
        int Quantity,
        long ElapsedMilliseconds,
        string Message
    );

    #endregion

    #region INSERT Masivo (SIN VALIDACIONES)

    /// <summary>
    /// Inserta usuarios masivamente OMITIENDO TODAS LAS VALIDACIONES
    /// ⚠️ SOLO PARA PRUEBAS DE RENDIMIENTO
    /// </summary>
    private static async Task<IResult> TestInsertUsers(
        UsersPerformanceRequestDto request,
        UserManager<NorthWindUser> userManager, // Cambio de tipo
        NorthWindMembershipContext dbContext)   // Cambio de tipo
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

        Console.WriteLine($"\n[Performance] Iniciando inserción de {request.Quantity:N0} usuarios...");
        Console.WriteLine($"[Performance] ⚠️ MODO RÁPIDO: Validaciones deshabilitadas");

        try
        {
            // 4. Cambiar la lista a NorthWindUser
            var usersToInsert = new List<NorthWindUser>();

            for (int i = 1; i <= request.Quantity; i++)
            {
                // Generar datos únicos
                string uniqueId = Guid.NewGuid().ToString()[..8];
                string email = $"perftest_{uniqueId}@test.com";
                string userName = $"perftest_{uniqueId}";

                // 5. Instanciar NorthWindUser en lugar de IdentityUser
                var user = new NorthWindUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userName,
                    Email = email,
                    // Propiedades específicas de NorthWindUser (rellenar con datos dummy)
                    FirstName = "PerfTest",
                    LastName = "User",
                    Cedula = "0000000000", // Valor dummy

                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedUserName = userName.ToUpper(),
                    NormalizedEmail = email.ToUpper(),
                    PasswordHash = "AQAAAAIAAYagAAAAEDummyHashForTestingPurposesOnly123456789"
                };

                usersToInsert.Add(user);

                // Batch insert cada 1000 usuarios
                if (usersToInsert.Count >= 1000)
                {
                    await BulkInsertUsers(dbContext, usersToInsert);
                    successCount += usersToInsert.Count;
                    usersToInsert.Clear();

                    Console.WriteLine($"[Performance] Progreso: {successCount:N0}/{request.Quantity:N0} usuarios insertados");
                }
            }

            // Insertar los usuarios restantes
            if (usersToInsert.Count > 0)
            {
                await BulkInsertUsers(dbContext, usersToInsert);
                successCount += usersToInsert.Count;
                Console.WriteLine($"[Performance] Progreso: {successCount:N0}/{request.Quantity:N0} usuarios insertados");
            }

            stopwatch.Stop();

            var message = $"✓ Se insertaron {successCount:N0} usuarios exitosamente";
            Console.WriteLine($"[Performance] {message} en {stopwatch.ElapsedMilliseconds:N0} ms");

            return Results.Ok(new UsersPerformanceResultDto(
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

            return Results.Ok(new UsersPerformanceResultDto(
                "INSERT",
                successCount,
                stopwatch.ElapsedMilliseconds,
                errorMsg
            ));
        }
    }

    /// <summary>
    /// Inserción masiva directa a BD (OMITE VALIDACIONES DE IDENTITY)
    /// </summary>
    // 6. Actualizar firma del método auxiliar
    private static async Task BulkInsertUsers(NorthWindMembershipContext context, List<NorthWindUser> users)
    {
        try
        {
            // Inserción directa sin validaciones de Identity
            await context.Set<NorthWindUser>().AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Performance] ⚠️ Error en bulk insert: {ex.Message}");
            throw;
        }
    }

    #endregion

    #region SELECT Masivo con Paginación

    /// <summary>
    /// Consulta usuarios masivamente usando paginación automática
    /// </summary>
    private static async Task<IResult> TestSelectUsers(
        UsersPerformanceRequestDto request,
        UserManager<NorthWindUser> userManager) // Cambio de tipo
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
        const int PAGE_SIZE = 100;
        bool hasMoreData = true;
        int pagesProcessed = 0;

        Console.WriteLine($"\n[Performance] Iniciando consulta de {request.Quantity:N0} usuarios...");
        Console.WriteLine($"[Performance] Estrategia: Páginas de {PAGE_SIZE} usuarios hasta alcanzar objetivo");

        try
        {
            while (hasMoreData && totalFetched < request.Quantity)
            {
                int skip = (currentPage - 1) * PAGE_SIZE;
                int remainingNeeded = request.Quantity - totalFetched;
                int take = Math.Min(PAGE_SIZE, remainingNeeded);

                Console.WriteLine($"[Performance] → Solicitando página {currentPage} (skip: {skip}, take: {take})...");

                var usersPage = await userManager.Users
                    .OrderBy(u => u.Email)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                if (usersPage.Any())
                {
                    int fetchedInPage = usersPage.Count;
                    totalFetched += fetchedInPage;
                    pagesProcessed++;

                    Console.WriteLine($"[Performance]   ✓ Recibidos: {fetchedInPage} usuarios");
                    Console.WriteLine($"[Performance]   📊 Total acumulado: {totalFetched:N0}/{request.Quantity:N0}");

                    if (totalFetched >= request.Quantity)
                    {
                        hasMoreData = false;
                        Console.WriteLine($"[Performance] ✓ OBJETIVO ALCANZADO: {totalFetched:N0} usuarios obtenidos");
                    }
                    else if (fetchedInPage < take)
                    {
                        hasMoreData = false;
                        Console.WriteLine($"[Performance] ⚠ NO HAY MÁS DATOS EN BD. Total disponible: {totalFetched:N0}");
                    }
                    else
                    {
                        currentPage++;
                    }
                }
                else
                {
                    hasMoreData = false;
                    Console.WriteLine($"[Performance] ⚠ PÁGINA VACÍA. Fin de datos en BD.");
                }

                Console.WriteLine();

                if (hasMoreData)
                {
                    await Task.Delay(10);
                }
            }

            stopwatch.Stop();

            string message;
            if (totalFetched >= request.Quantity)
            {
                message = $"✓ Se consultaron {totalFetched:N0} usuarios exitosamente en {pagesProcessed} páginas";
            }
            else
            {
                message = $"⚠ Se consultaron {totalFetched:N0} de {request.Quantity:N0} solicitados (Total disponible en BD) en {pagesProcessed} páginas";
            }

            Console.WriteLine($"[Performance] {message} - Tiempo: {stopwatch.ElapsedMilliseconds:N0} ms");

            return Results.Ok(new UsersPerformanceResultDto(
                "SELECT",
                totalFetched,
                stopwatch.ElapsedMilliseconds,
                message
            ));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var errorMsg = $"✗ Error después de consultar {totalFetched:N0} usuarios: {ex.Message}";
            Console.WriteLine($"[Performance] {errorMsg}");

            return Results.Ok(new UsersPerformanceResultDto(
                "SELECT",
                totalFetched,
                stopwatch.ElapsedMilliseconds,
                errorMsg
            ));
        }
    }

    #endregion
}