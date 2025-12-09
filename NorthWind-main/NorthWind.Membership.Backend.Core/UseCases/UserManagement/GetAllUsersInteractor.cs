using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement
{
    internal class GetAllUsersInteractor(
        IMembershipService membershipService,
        IGetAllUsersOutputPort presenter) : IGetAllUsersInputPort
    {
        public async Task Handle(int pageNumber, int pageSize)
        {
            var pagedUsers = await membershipService.GetAllUsers(pageNumber, pageSize);
            await presenter.Handle(pagedUsers);
        }
    }
}
