using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // <--- Asegúrate de tener este using
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Northwind.Sales.WebApi.Options;
using NorthWind.Sales.Backend.BusinessObjects.Interfaces.Common;

namespace Northwind.Sales.WebApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly SalesJwtOptions _options;

        public JwtService(IOptions<SalesJwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(string customerId, string email, string name)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customerId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("Name", name),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var token = new JwtSecurityToken(
                issuer: _options.ValidIssuer,
                audience: _options.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_options.ExpireInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}