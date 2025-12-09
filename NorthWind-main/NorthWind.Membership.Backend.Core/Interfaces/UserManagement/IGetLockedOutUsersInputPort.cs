namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    public interface IGetLockedOutUsersInputPort
    {
        Task Handle(int pageNumber, int pageSize);
    }
}
