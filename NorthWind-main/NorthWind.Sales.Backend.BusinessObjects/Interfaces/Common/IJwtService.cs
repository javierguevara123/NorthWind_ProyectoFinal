namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Common
{
    public interface IJwtService
    {
        string GenerateToken(string customerId, string email, string name);
    }
}
