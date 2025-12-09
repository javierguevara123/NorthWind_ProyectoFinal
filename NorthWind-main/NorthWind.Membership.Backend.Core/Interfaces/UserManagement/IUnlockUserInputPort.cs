using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    public interface IUnlockUserInputPort
    {
        Task Handle(UnlockUserDto unlockData);
    }
}
