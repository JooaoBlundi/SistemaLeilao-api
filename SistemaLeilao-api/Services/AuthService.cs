using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaLeilao_api.Data;
using SistemaLeilao_api.Models;
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_api.Entities;
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

        public async Task<string?> AuthenticateAsync(LoginModel model)
        {
            AddNotifications(new Flunt.Validations.Contract<LoginModel>()
                .Requires()
                .IsNotNullOrEmpty(model.Email, "Email", "Email não pode ser vazio.")
                .IsEmail(model.Email, "Email", "Email inválido.")
                .IsNotNullOrEmpty(model.Senha, "Senha", "Senha não pode ser vazia.")
            );

            if (!IsValid) return null; 

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                AddNotification("Auth", "Usuário ou senha inválidos.");
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Senha, user.Senha))
            {
                AddNotification("Auth", "Usuário ou senha inválidos.");
                return null;
            }

            TimeSpan tokenExpiration = TimeSpan.FromHours(8); 
            if (model.ManterConectado)
            {
                tokenExpiration = TimeSpan.FromHours(1); 
                user.ManterConectadoAte = DateTime.UtcNow.Add(tokenExpiration);
            }
            else
            {
                user.ManterConectadoAte = null; 
            }

            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync(); 

            return GenerateJwtToken(user, tokenExpiration);
        }

        private string GenerateJwtToken(Usuario user, TimeSpan expiresIn)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nome), 
                    new Claim(ClaimTypes.Email, user.Email) 
                }),
                Expires = DateTime.UtcNow.Add(expiresIn), 
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

