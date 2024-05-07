using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entity;
using Repository.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Repository.Service
{
    public class AuthServiceRepositoryLayer : IAuthServiceRepositoryLayer
    {
        private readonly IConfiguration _configuration;

        public AuthServiceRepositoryLayer(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GenerateJwtToken(Registration user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];


            if (key.Length < 32)
            {
                throw new ArgumentException("JWT secret key must be at least 256 bits (32 bytes)");
            }

            var claims = new[]
            {
       // new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("UserId",user.UserId.ToString()),
        new Claim("FirstName",user.FirstName),
        new Claim("LastName",user.LastName)

        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}