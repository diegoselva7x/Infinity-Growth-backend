using DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppLogic.Services
{
    public class Jwt_Service
    {
        private readonly IConfiguration _configuration;

        public Jwt_Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Usuarios user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Correo),
                new Claim("Correo", user.Correo.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Nombre", user.Nombre),
                new Claim("Apellido1", user.Apellido1),
                new Claim("Apellido2", user.Apellido2),
                new Claim("TipoUsuario", user.TipoUsuario.ToString()),
                new Claim("FechaNacimiento", user.FechaNacimiento.ToShortDateString()),
                new Claim("Estado", user.Estado.ToString()),
                new Claim("isPasswordTemp", user.isPasswordTemp.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
