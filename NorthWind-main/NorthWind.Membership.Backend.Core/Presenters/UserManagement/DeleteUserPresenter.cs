using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Extensions;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Membership.Backend.Core.Presenters.UserManagement
{
    internal class DeleteUserPresenter : IDeleteUserOutputPort
    {
        public IResult Result { get; private set; }

        public Task Handle(Result<IEnumerable<ValidationError>> result)
        {
            result.HandleResult(
                errors =>
                {
                    Result = Results.Problem(
                        errors.ToProblemDetails(
                            "Error al eliminar usuario",
                            "No se pudo eliminar el usuario",
                            nameof(DeleteUserPresenter)));
                },
                () =>
                {
                    Result = Results.Ok(new { message = "Usuario eliminado exitosamente" });
                });
            return Task.CompletedTask;
        }
    }
}
