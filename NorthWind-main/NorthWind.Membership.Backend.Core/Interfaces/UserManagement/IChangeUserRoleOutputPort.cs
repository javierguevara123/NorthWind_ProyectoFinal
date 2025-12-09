using Microsoft.AspNetCore.Http;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    internal interface IChangeUserRoleOutputPort
    {
        IResult Result { get; }
        Task Handle(Result<IEnumerable<ValidationError>> result);
    }
}
