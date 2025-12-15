namespace Northwind.Sales.WebApi.Options
{
    public class SalesJwtOptions
    {
        public const string SectionName = "JwtOptions";
        public string SecurityKey { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = string.Empty;
        public int ExpireInMinutes { get; set; }
    }
}
