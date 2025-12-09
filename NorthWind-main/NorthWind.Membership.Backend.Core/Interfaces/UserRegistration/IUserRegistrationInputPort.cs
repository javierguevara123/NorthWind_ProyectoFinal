using NorthWind.Membership.Entities.Dtos.UserRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserRegistration
{
    internal interface IUserRegistrationInputPort
    {
        Task Handle(UserRegistrationDto userData);
    }
}
