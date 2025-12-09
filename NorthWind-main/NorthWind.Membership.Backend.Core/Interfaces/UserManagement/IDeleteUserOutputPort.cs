using Microsoft.AspNetCore.Http;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    internal interface IDeleteUserOutputPort
    {
        IResult Result { get; }
        Task Handle(Result<IEnumerable<ValidationError>> result);
    }
}
