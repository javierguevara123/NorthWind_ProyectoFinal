namespace NorthWind.Sales.Entities.Dtos.Customers.Login
{
    public record CustomerCredentialDto(string Id, string Name, string Email, string? HashedPassword);
}