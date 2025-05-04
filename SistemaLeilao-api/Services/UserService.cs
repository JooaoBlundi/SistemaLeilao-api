using Flunt.Notifications;
using Flunt.Validations;
using Microsoft.EntityFrameworkCore;
using SistemaLeilao_api.Data;
using SistemaLeilao_api.Models;
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_api.Entities;
using System.Threading.Tasks;
using SistemaLeilao_web.Model;

namespace SistemaLeilao_api.Services
{
    public class UserService : Notifiable<Notification>, IUserService
    {
        private readonly LeilaoDbContext _context;

        public UserService(LeilaoDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> RegisterUserAsync(UsuarioModel model)
        {

            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            {
                AddNotification("Email", "Este email já está cadastrado.");
                return null;
            }
            
            if (await _context.Usuarios.AnyAsync(u => u.Cpf == model.Cpf.Replace(".", "").Replace("-", "")))
            {
                 AddNotification("Cpf", "Este CPF já está cadastrado.");
                 return null;
            }

            var hashedPassword = AuthService.HashPassword(model.Senha);

            var newUser = new Usuario
            {
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Email = model.Email,
                Senha = hashedPassword, 
                Cpf = model.Cpf.Replace(".", "").Replace("-", ""), 
                DataNascimento = model.DataNascimento,
                Sexo = model.Sexo,
                Endereco = model.Endereco,
                Cidade = model.Cidade,
                Uf = model.Uf,
                Agencia = model.Agencia,
                Conta = model.Conta,
                ChavePix = model.ChavePix,
                DataCadastro = DateTime.UtcNow
            };

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser; 
        }

        public void ValidateUserModel(UsuarioModel model)
        {
            AddNotifications(new Contract<UsuarioModel>()
                .Requires()
                .IsNotNullOrEmpty(model.Nome, "Nome", "Nome é obrigatório")
                .IsNotNullOrEmpty(model.Sobrenome, "Sobrenome", "Sobrenome é obrigatório")
                .IsNotNullOrEmpty(model.Email, "Email", "Email é obrigatório")
                .IsEmail(model.Email, "Email", "Email inválido")
                .IsNotNullOrEmpty(model.Senha, "Senha", "Senha é obrigatória")
                .IsGreaterThan(model.Senha, 5, "Senha", "Senha deve ter no mínimo 6 caracteres")
                .IsNotNullOrEmpty(model.Cpf, "Cpf", "CPF é obrigatório")
                .IsTrue(model.Cpf != null && model.Cpf.Replace(".", "").Replace("-", "").Length == 11, "Cpf", "CPF deve ter 11 dígitos")
            );
        }
    }
}

