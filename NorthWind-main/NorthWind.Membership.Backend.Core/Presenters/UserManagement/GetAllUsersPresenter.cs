using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.Common;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Presenters.UserManagement
{
    internal class GetAllUsersPresenter : IGetAllUsersOutputPort
    {
        public IResult Result { get; private set; }

        public Task Handle(PagedResultDto<UserInfoDto> pagedUsers)
        {
            Result = Results.Ok(pagedUsers);
            return Task.CompletedTask;
        }
    }
}
