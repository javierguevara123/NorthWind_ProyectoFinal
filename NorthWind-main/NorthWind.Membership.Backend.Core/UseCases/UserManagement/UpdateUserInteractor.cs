using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement
{
    internal class UpdateUserInteractor(
        IMembershipService membershipService,
        IUpdateUserOutputPort presenter) : IUpdateUserInputPort
    {
        public async Task Handle(UpdateUserDto updateData)
        {
            var result = await membershipService.UpdateUser(
                updateData.Email,
                updateData.FirstName,
                updateData.LastName,
                updateData.Cedula,
                updateData.NewPassword,
                updateData.CurrentUserEmail);

            await presenter.Handle(result);
        }
    }
}
