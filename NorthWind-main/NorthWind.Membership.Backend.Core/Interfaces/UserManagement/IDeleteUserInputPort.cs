using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    internal interface IDeleteUserInputPort
    {
        Task Handle(DeleteUserDto deleteData);
    }
}
