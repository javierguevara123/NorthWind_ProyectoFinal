using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    public interface IGetUserByIdInputPort
    {
        Task Handle(GetUserByIdDto data);
    }
}
