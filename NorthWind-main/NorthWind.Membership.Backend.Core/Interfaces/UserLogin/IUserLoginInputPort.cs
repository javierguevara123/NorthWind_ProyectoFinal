using NorthWind.Membership.Entities.UserLogin;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserLogin
{
    internal interface IUserLoginInputPort
    {
        Task Handle(UserCredentialsDto userData);
    }
}
