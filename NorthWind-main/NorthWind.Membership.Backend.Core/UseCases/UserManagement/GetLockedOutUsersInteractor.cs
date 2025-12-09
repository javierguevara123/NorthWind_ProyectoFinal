using NorthWind.Membership.Backend.Core.Interfaces.Common;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;

namespace NorthWind.Membership.Backend.Core.UseCases.UserManagement
{
    internal class GetLockedOutUsersInteractor(
        IMembershipService membershipService,
        IGetLockedOutUsersOutputPort presenter) : IGetLockedOutUsersInputPort
    {
        public async Task Handle(int pageNumber, int pageSize)
        {
            var pagedUsers = await membershipService.GetLockedOutUsers(pageNumber, pageSize);
            await presenter.Handle(pagedUsers);
        }
    }
}
