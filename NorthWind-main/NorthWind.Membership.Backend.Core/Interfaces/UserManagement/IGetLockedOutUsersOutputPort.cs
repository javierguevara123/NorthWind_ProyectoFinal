using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Entities.Dtos.Common;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    internal interface IGetLockedOutUsersOutputPort
    {
        IResult Result { get; }
        Task Handle(PagedResultDto<UserInfoDto> pagedUsers);
    }
}
