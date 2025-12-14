namespace NorthWind.Sales.Backend.BusinessObjects.Entities
{
    /// <summary>
    /// Entidad simple de dominio para Customer.
    /// Sin reglas de negocio complejas.
    /// </summary>
    public class Customer
    {
        public string Id { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }

        public string Email { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;

        public byte[]? ProfilePicture { get; set; }

    }
}