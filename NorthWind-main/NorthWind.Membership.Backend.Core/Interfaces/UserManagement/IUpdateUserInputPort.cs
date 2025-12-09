using NorthWind.Membership.Entities.Dtos.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    internal interface IUpdateUserInputPort
    {
        Task Handle(UpdateUserDto updateData);
    }
}
