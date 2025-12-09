using Microsoft.AspNetCore.Http;
using NorthWind.Membership.Backend.Core.Interfaces.UserManagement;
using NorthWind.Membership.Entities.Dtos.UserManagement;

namespace NorthWind.Membership.Backend.Core.Presenters.UserManagement
{
    public class GetUserByIdPresenter : IGetUserByIdOutputPort
    {
        public IResult Result { get; private set; }

        public void Handle(GetUserByIdResponse response)
        {
            if (response.Success)
            {
                Result = Results.Ok(new
                {
                    success = response.Success,
                    message = response.Message,
                    user = response.User
                });
            }
            else
            {
                var statusCode = response.Message.Contains("permisos") ? 403 : 404;
                Result = Results.Json(
                    new
                    {
                        success = response.Success,
                        message = response.Message,
                        errors = response.Errors
                    },
                    statusCode: statusCode
                );
            }
        }
    }
}
