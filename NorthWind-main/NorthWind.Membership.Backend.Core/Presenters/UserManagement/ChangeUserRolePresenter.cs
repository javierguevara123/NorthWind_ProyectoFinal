using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Extensions;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Membership.Backend.Core.Presenters.UserManagement
{
    internal class ChangeUserRolePresenter : IChangeUserRoleOutputPort
    {
        public IResult Result { get; private set; }

        public Task Handle(Result<IEnumerable<ValidationError>> result)
        {
            result.HandleResult(
                errors =>
                {
                    Result = Results.Problem(
                        errors.ToProblemDetails(
                            "Error al cambiar rol",
                            "No se pudo cambiar el rol del usuario",
                            nameof(ChangeUserRolePresenter)));
                },
                () =>
                {
                    Result = Results.Ok(new { message = "Rol de usuario actualizado exitosamente" });
                });
            return Task.CompletedTask;
        }
    }
}
