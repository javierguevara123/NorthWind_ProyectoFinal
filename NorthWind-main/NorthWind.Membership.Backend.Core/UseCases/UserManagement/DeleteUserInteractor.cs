using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement
{
    internal class DeleteUserInteractor(
        IMembershipService membershipService,
        IDeleteUserOutputPort presenter) : IDeleteUserInputPort
    {
        public async Task Handle(DeleteUserDto deleteData)
        {
            var result = await membershipService.DeleteUser(
                deleteData.Email,
                deleteData.CurrentUserEmail);

            await presenter.Handle(result);
        }
    }
}
