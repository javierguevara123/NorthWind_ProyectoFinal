namespace NorthWind.Sales.Entities.Dtos.Customers.UpdateCustomer
{
    public class UpdateCustomerDto(
        string customerid,
        string name,
        decimal currentBalance,
        string email,
        string cedula,
        string? profilePictureBase64,
        string? password) // <--- NUEVO: Contraseña opcional
    {
        public string CustomerId => customerid;
        public string Name => name;
        public decimal CurrentBalance => currentBalance;
        public string Email => email;
        public string Cedula => cedula;
        public string? ProfilePictureBase64 => profilePictureBase64;
        public string? Password => password; // <--- Getter
    }
}