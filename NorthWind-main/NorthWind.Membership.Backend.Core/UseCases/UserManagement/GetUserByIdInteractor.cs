using Microsoft.AspNetCore.Identity;
using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement
{
    public class GetUserByIdInteractor : IGetUserByIdInputPort
    {
        private readonly IGetUserByIdOutputPort _outputPort;
        private readonly IMembershipService _membershipService;

        public GetUserByIdInteractor(
            IGetUserByIdOutputPort outputPort,
            IMembershipService membershipService)
        {
            _outputPort = outputPort;
            _membershipService = membershipService;
        }

        public async Task Handle(GetUserByIdDto data)
        {
            var response = new GetUserByIdResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(data.UserId))
                {
                    response.Success = false;
                    response.Message = "ID de usuario no proporcionado.";
                    response.Errors.Add("El ID es requerido.");
                    _outputPort.Handle(response);
                    return;
                }

                var user = await _membershipService.GetUserById(data.UserId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Usuario no encontrado.";
                    response.Errors.Add($"No existe un usuario con el ID: {data.UserId}");
                    _outputPort.Handle(response);
                    return;
                }

                // Verificar permisos: solo Admin, SuperUser o el propio usuario pueden ver la info
                var currentUser = await _membershipService.GetUserByEmail(data.CurrentUserEmail);
                var isAdmin = data.CurrentUserRole == "Administrator" || data.CurrentUserRole == "SuperUser";
                var isSameUser = currentUser?.Id == user.Id;

                if (!isAdmin && !isSameUser)
                {
                    response.Success = false;
                    response.Message = "No tienes permisos para ver este usuario.";
                    response.Errors.Add("Acceso denegado.");
                    _outputPort.Handle(response);
                    return;
                }

                response.Success = true;
                response.Message = "Usuario obtenido exitosamente.";
                response.User = new UserDetailDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Cedula = user.Cedula,
                    Role = user.Roles.FirstOrDefault() ?? "Employee",
                    IsLockedOut = user.IsLockedOut,
                    LockoutEnd = user.LockoutEnd?.UtcDateTime,
                    AccessFailedCount = user.AccessFailedCount
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error al obtener el usuario.";
                response.Errors.Add(ex.Message);
            }

            _outputPort.Handle(response);
        }
    }
}
