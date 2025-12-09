using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Extensions;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Membership.Backend.Core.Presenters.UserManagement
{
    internal class UpdateUserPresenter : IUpdateUserOutputPort
    {
        public IResult Result { get; private set; }

        public Task Handle(Result<IEnumerable<ValidationError>> result)
        {
            result.HandleResult(
                errors =>
                {
                    Result = Results.Problem(
                        errors.ToProblemDetails(
                            "Error al actualizar usuario",
                            "No se pudo actualizar el usuario",
                            nameof(UpdateUserPresenter)));
                },
                () =>
                {
                    Result = Results.Ok(new { message = "Usuario actualizado exitosamente" });
                });
            return Task.CompletedTask;
        }
    }
}
