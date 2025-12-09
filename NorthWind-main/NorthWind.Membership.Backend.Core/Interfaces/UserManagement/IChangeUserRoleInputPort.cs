using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    public interface IChangeUserRoleInputPort
    {
        Task Handle(ChangeUserRoleDto roleData);
    }
}
