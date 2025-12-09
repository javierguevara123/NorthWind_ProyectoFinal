namespace NorthWind.Membership.Backend.Core.Interfaces.UserManagement
{
    public interface IGetAllUsersInputPort
    {
        Task Handle(int pageNumber, int pageSize);
    }
}
