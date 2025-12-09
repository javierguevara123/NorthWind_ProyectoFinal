using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement
{
    internal class UnlockUserInteractor(
        IMembershipService membershipService,
        IUnlockUserOutputPort presenter) : IUnlockUserInputPort
    {
        public async Task Handle(UnlockUserDto unlockData)
        {
            var result = await membershipService.UnlockUser(unlockData.Email);
            await presenter.Handle(result);
        }
    }
}
