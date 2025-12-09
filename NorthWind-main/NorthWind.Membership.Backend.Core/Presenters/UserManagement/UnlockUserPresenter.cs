using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Extensions;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Membership.Backend.Core.Presenters.UserManagement
{
    internal class UnlockUserPresenter : IUnlockUserOutputPort
    {
        public IResult Result { get; private set; }

        public Task Handle(Result<IEnumerable<ValidationError>> result)
        {
            result.HandleResult(
                errors =>
                {
                    Result = Results.Problem(
                        errors.ToProblemDetails(
                            "Error al desbloquear usuario",
                            "No se pudo desbloquear el usuario",
                            nameof(UnlockUserPresenter)));
                },
                () =>
                {
                    Result = Results.Ok(new { message = "Usuario desbloqueado exitosamente" });
                });
            return Task.CompletedTask;
        }
    }
}
