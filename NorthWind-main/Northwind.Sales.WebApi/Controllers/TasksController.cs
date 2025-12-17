using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;
using NorthWind.Sales.Backend.Repositories.Entities; // Tu DbContext real

namespace Microsoft.AspNetCore.Builder;

public static class TasksController
{
    public static WebApplication UseTasksController(this WebApplication app)
    {
        // --- 1. GET LISTA (Sin seguridad) ---
        app.MapGet("/api/tasks", async ([FromServices] NorthWindContext db, int PageNumber = 1, int PageSize = 100) =>
        {
            var tareas = await db.Set<TaskEntity>()
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Devolvemos envuelto en "items" para que Flutter lo lea fácil
            return Results.Ok(new { items = tareas });
        })
        .WithName("GetTasks")
        .Produces(StatusCodes.Status200OK);

        // --- 2. GET POR ID (Sin seguridad) ---
        app.MapGet("/api/tasks/{id}", async ([FromServices] NorthWindContext db, int id) =>
        {
            var tarea = await db.Set<TaskEntity>().FindAsync(id);
            return tarea is not null ? Results.Ok(tarea) : Results.NotFound();
        })
        .WithName("GetTaskById");

        // --- 3. CREAR (Sin validaciones, guarda lo que venga) ---
        app.MapPost("/api/tasks", async ([FromServices] NorthWindContext db, [FromBody] TaskEntity nuevaTarea) =>
        {
            db.Set<TaskEntity>().Add(nuevaTarea);
            await db.SaveChangesAsync();
            return Results.Created($"/api/tasks/{nuevaTarea.Id}", nuevaTarea);
        })
        .WithName("CreateTask");

        // --- 4. ACTUALIZAR (Directo a la base) ---
        app.MapPut("/api/tasks/{id}", async ([FromServices] NorthWindContext db, int id, [FromBody] TaskEntity tareaEditada) =>
        {
            var tarea = await db.Set<TaskEntity>().FindAsync(id);
            if (tarea is null) return Results.NotFound();

            tarea.Title = tareaEditada.Title;
            tarea.Description = tareaEditada.Description;
            tarea.IsCompleted = tareaEditada.IsCompleted;

            await db.SaveChangesAsync();
            return Results.Ok(tarea);
        })
        .WithName("UpdateTask");

        // --- 5. ELIMINAR ---
        app.MapDelete("/api/tasks/{id}", async ([FromServices] NorthWindContext db, int id) =>
        {
            var tarea = await db.Set<TaskEntity>().FindAsync(id);
            if (tarea is not null)
            {
                db.Set<TaskEntity>().Remove(tarea);
                await db.SaveChangesAsync();
            }
            return Results.Ok();
        })
        .WithName("DeleteTask");

        return app;
    }
}