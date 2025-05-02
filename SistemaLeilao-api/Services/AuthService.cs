using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaLeilao_api.Data;
using SistemaLeilao_api.DTOs;
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_api.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net; // Add this

namespace SistemaLeilao_api.Services
{
    public class AuthService : Notifiable<Notification>, IAuthService
    {
        private readonly LeilaoDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(LeilaoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string?> AuthenticateAsync(LoginDto loginDto)
        {
            // Basic validation using Flunt
            AddNotifications(new Flunt.Validations.Contract<LoginDto>()
                .Requires()
                .IsNotNullOrEmpty(loginDto.Email, "Email", "Email não pode ser vazio.")
                .IsEmail(loginDto.Email, "Email", "Email inválido.")
                .IsNotNullOrEmpty(loginDto.Senha, "Senha", "Senha não pode ser vazia.")
            );

            if (!IsValid) return null; // Return null if basic validation fails

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                AddNotification("Auth", "Usuário ou senha inválidos.");
                return null;
            }

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Senha, user.Senha))
            {
                AddNotification("Auth", "Usuário ou senha inválidos.");
                return null;
            }

            // If authentication is successful, generate JWT token
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Usuario user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nome), // Add user name claim
                    new Claim(ClaimTypes.Email, user.Email) // Add email claim
                    // Add other claims as needed (e.g., roles)
                }),
                Expires = DateTime.UtcNow.AddHours(8), // Token expiration time (e.g., 8 hours)
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Helper method for hashing password (to be used in user registration service)
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

