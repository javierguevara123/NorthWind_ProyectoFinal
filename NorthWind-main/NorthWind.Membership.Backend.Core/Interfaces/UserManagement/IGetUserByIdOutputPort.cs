using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    public interface IGetUserByIdOutputPort
    {
        IResult Result { get; }
        void Handle(GetUserByIdResponse response);
    }
}
