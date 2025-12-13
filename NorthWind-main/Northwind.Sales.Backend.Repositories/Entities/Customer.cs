namespace NorthWind.Sales.Backend.Repositories.Entities
{
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Email { get; set; }
        public string Cedula { get; set; }
        public string HashedPassword { get; set; }
    }
}
