using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace Microsoft.AspNetCore.Builder;

public static class TasksUserController
{
    public static WebApplication UseTasksUserController(this WebApplication app)
    {
        // 1. GET: /api/taskuser
        app.MapGet("/api/taskuser", async ([FromServices] NorthWindContext db, ClaimsPrincipal user) =>
        {
            var currentEmail = user.Identity?.Name;
            if (string.IsNullOrEmpty(currentEmail)) return Results.Unauthorized();

            var tareas = await db.TaskUsers
                                 .Where(t => t.UserId == currentEmail)
                                 .ToListAsync();

            return Results.Ok(new { items = tareas });
        })
        .RequireAuthorization();

        // 2. POST: /api/taskuser
        app.MapPost("/api/taskuser", async ([FromServices] NorthWindContext db, ClaimsPrincipal user, [FromBody] TaskUser task) =>
        {
            var currentEmail = user.Identity?.Name;
            if (string.IsNullOrEmpty(currentEmail)) return Results.Unauthorized();

            task.UserId = currentEmail;

            db.TaskUsers.Add(task);
            await db.SaveChangesAsync();
            return Results.Ok(task);
        })
        .RequireAuthorization();

        // 3. PUT: /api/taskuser/{id}
        app.MapPut("/api/taskuser/{id}", async ([FromServices] NorthWindContext db, ClaimsPrincipal user, int id, [FromBody] TaskUser taskInput) =>
        {
            var currentEmail = user.Identity?.Name;

            var task = await db.TaskUsers
                               .FirstOrDefaultAsync(t => t.Id == id && t.UserId == currentEmail);

            if (task is null) return Results.NotFound();

            task.Title = taskInput.Title;
            task.Type = taskInput.Type;
            task.IsCompleted = taskInput.IsCompleted;

            await db.SaveChangesAsync();
            return Results.Ok(task);
        })
        .RequireAuthorization();

        // 4. DELETE: /api/taskuser/{id}
        app.MapDelete("/api/taskuser/{id}", async ([FromServices] NorthWindContext db, ClaimsPrincipal user, int id) =>
        {
            var currentEmail = user.Identity?.Name;

            var task = await db.TaskUsers
                               .FirstOrDefaultAsync(t => t.Id == id && t.UserId == currentEmail);

            if (task is null) return Results.NotFound();

            db.TaskUsers.Remove(task);
            await db.SaveChangesAsync();
            return Results.Ok();
        })
        .RequireAuthorization();

        return app;
    }
}