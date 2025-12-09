using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement
{
    internal class ChangeUserRoleInteractor(
        IMembershipService membershipService,
        IChangeUserRoleOutputPort presenter) : IChangeUserRoleInputPort
    {
        public async Task Handle(ChangeUserRoleDto roleData)
        {
            var result = await membershipService.ChangeUserRole(roleData.Email, roleData.NewRole);
            await presenter.Handle(result);
        }
    }
}
