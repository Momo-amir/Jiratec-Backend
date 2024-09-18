using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration; // Make sure to include this namespace

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly string _jwtSecret;

        // Inject IConfiguration to access appsettings.json values
        public UserService(IConfiguration? configuration)
        {
            _passwordHasher = new PasswordHasher<User>();

            // Access the secret key from appsettings.json
            _jwtSecret = configuration?["Jwt:Secret"] ?? throw new System.ArgumentNullException(nameof(configuration), "JWT secret is not configured.");
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Use the secret key from appsettings.json
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Set token expiration as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
