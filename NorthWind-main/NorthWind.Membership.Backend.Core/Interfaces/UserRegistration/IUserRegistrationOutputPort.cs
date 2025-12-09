using Microsoft.AspNetCore.Http;
using NorthWind.Result.Entities;
using NorthWind.Validation.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserRegistration
{
    internal interface IUserRegistrationOutputPort
    {
        IResult Result { get; }
        Task Handle(Result<IEnumerable<ValidationError>> userRegistrationResult);
    }

}
