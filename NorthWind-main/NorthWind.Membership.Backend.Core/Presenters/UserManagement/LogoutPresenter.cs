using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Responses;

namespace NorthWind.Membership.Backend.Core.Presenters.UserManagement
{
    public class LogoutPresenter : ILogoutOutputPort
    {
        public IResult Result { get; private set; }

        public void Handle(LogoutResponse response)
        {
            if (response.Success)
            {
                Result = Results.Ok(new
                {
                    success = response.Success,
                    message = response.Message
                });
            }
            else
            {
                Result = Results.BadRequest(new
                {
                    success = response.Success,
                    message = response.Message,
                    errors = response.Errors
                });
            }
        }
    }
}
