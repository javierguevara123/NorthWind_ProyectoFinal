using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;
using NorthWind.Membership.Entities.ValueObjects;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Builder;
internal static class UserManagementController
{
    public static WebApplication UseUserManagementController(this WebApplication app)
    {
        // GET: Obtener todos los usuarios (Solo Admin y SuperUser)
        app.MapGet(Endpoints.GetAllUsers,
    [Authorize(Roles = "Administrator,SuperUser")]
        async (IGetAllUsersInputPort inputPort,
    IGetAllUsersOutputPort presenter,
    int pageNumber = 1,
    int pageSize = 10) =>
    {
        await inputPort.Handle(pageNumber, pageSize);
        return presenter.Result;
    })
    .RequireAuthorization();

        // GET: Obtener usuario por ID
        app.MapGet(Endpoints.GetUserById,
            [Authorize]
        async (string userId,
            HttpContext httpContext,
            IGetUserByIdInputPort inputPort,
            IGetUserByIdOutputPort presenter) =>
            {
                // Obtener el email del usuario autenticado
                var currentUserEmail = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var currentUserRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                var getUserData = new GetUserByIdDto
                {
                    UserId = userId,
                    CurrentUserEmail = currentUserEmail,
                    CurrentUserRole = currentUserRole
                };

                await inputPort.Handle(getUserData);
                return presenter.Result;
            })
            .RequireAuthorization();

        // GET: Obtener usuarios bloqueados (Solo Admin y SuperUser)
        app.MapGet(Endpoints.GetLockedOutUsers,
            [Authorize(Roles = "Administrator,SuperUser")]
        async (IGetLockedOutUsersInputPort inputPort,
            IGetLockedOutUsersOutputPort presenter,
            int pageNumber = 1,
            int pageSize = 10) =>
            {
                await inputPort.Handle(pageNumber, pageSize);
                return presenter.Result;
            })
            .RequireAuthorization();

        // POST: Desbloquear usuario (Solo Admin y SuperUser)
        app.MapPost(Endpoints.UnlockUser,
            [Authorize(Roles = "Administrator,SuperUser")]
        async (UnlockUserDto unlockData,
            IUnlockUserInputPort inputPort,
            IUnlockUserOutputPort presenter) =>
            {
                await inputPort.Handle(unlockData);
                return presenter.Result;
            })
            .RequireAuthorization();

        // POST: Cambiar rol de usuario (Solo SuperUser)
        app.MapPost(Endpoints.ChangeUserRole,
            [Authorize(Roles = "SuperUser")]
        async (ChangeUserRoleDto roleData,
            IChangeUserRoleInputPort inputPort,
            IChangeUserRoleOutputPort presenter) =>
            {
                await inputPort.Handle(roleData);
                return presenter.Result;
            })
            .RequireAuthorization();

        // PUT: Actualizar usuario
        app.MapPut(Endpoints.UpdateUser,
            [Authorize]
        async (
            [FromForm] string email,
            [FromForm] string firstName,
            [FromForm] string lastName,
            [FromForm] string cedula,
            [FromForm] string? newPassword, // Puede ser nulo o vacío
            IFormFile? profilePicture,      // El archivo de imagen
            HttpContext httpContext,
            IUpdateUserInputPort inputPort,
            IUpdateUserOutputPort presenter) =>
            {
                // 1. Procesar la imagen si existe
                byte[]? pictureBytes = null;
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await profilePicture.CopyToAsync(memoryStream);
                    pictureBytes = memoryStream.ToArray();
                }

                // 2. Obtener el email del usuario autenticado
                var currentUserEmail = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

                // 3. Construir el DTO manualmente
                var updateData = new UpdateUserDto
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Cedula = cedula,
                    NewPassword = newPassword,
                    CurrentUserEmail = currentUserEmail
                };

                await inputPort.Handle(updateData);
                return presenter.Result;
            })
            .DisableAntiforgery() // Necesario a veces para subida de archivos en Minimal APIs simples
            .RequireAuthorization();

        // DELETE: Eliminar usuario
        app.MapDelete(Endpoints.DeleteUser,
            [Authorize]
        async (string email,
            HttpContext httpContext,
            IDeleteUserInputPort inputPort,
            IDeleteUserOutputPort presenter) =>
            {
                // Obtener el email del usuario autenticado desde el token JWT
                var currentUserEmail = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

                var deleteData = new DeleteUserDto
                {
                    Email = email,
                    CurrentUserEmail = currentUserEmail
                };

                await inputPort.Handle(deleteData);
                return presenter.Result;
            })
            .RequireAuthorization();

        // POST: Logout - Invalidar token
        app.MapPost(Endpoints.Logout,
            [Authorize]
        async (HttpContext httpContext,
            ILogoutInputPort inputPort,
            ILogoutOutputPort presenter) =>
            {
                var currentUserEmail = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var logoutData = new LogoutDto
                {
                    Email = currentUserEmail,
                    Token = token
                };

                await inputPort.Handle(logoutData);
                return presenter.Result;
            })
            .RequireAuthorization();


        return app;
    }
}