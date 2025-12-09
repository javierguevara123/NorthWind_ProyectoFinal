using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Entities.Responses;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    public interface ILogoutOutputPort
    {
        IResult Result { get; }
        void Handle(LogoutResponse response);
    }
}
