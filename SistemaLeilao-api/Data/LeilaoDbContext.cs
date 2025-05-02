using Microsoft.EntityFrameworkCore;
using SistemaLeilao_api.Models;

namespace SistemaLeilao_api.Data
{
    public class LeilaoDbContext : DbContext
    {
        public LeilaoDbContext(DbContextOptions<LeilaoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Leilao> Leiloes { get; set; }
        public DbSet<Lance> Lances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints if not fully covered by annotations

            // Example: Ensure Email and CPF are unique in the Usuario table
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Cpf)
                .IsUnique();

            // Configure cascade delete behavior if necessary (default might be restrictive)
            // Example: Prevent cascade delete from Usuario to Leilao (Vendedor)
            modelBuilder.Entity<Leilao>()
                .HasOne(l => l.Vendedor)
                .WithMany(u => u.LeiloesVendidos)
                .HasForeignKey(l => l.VendedorId)
                .OnDelete(DeleteBehavior.Restrict); // Or SetNull if VendedorId is nullable

            // Example: Prevent cascade delete from Usuario to Leilao (Comprador)
             modelBuilder.Entity<Leilao>()
                .HasOne(l => l.Comprador)
                .WithMany(u => u.LeiloesComprados)
                .HasForeignKey(l => l.CompradorId)
                .OnDelete(DeleteBehavior.Restrict); // Or SetNull if CompradorId is nullable

            // Example: Cascade delete Lances if Leilao is deleted
            modelBuilder.Entity<Lance>()
                .HasOne(l => l.Leilao)
                .WithMany(le => le.Lances)
                .HasForeignKey(l => l.LeilaoId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Example: Restrict delete of Usuario if they have Lances
             modelBuilder.Entity<Lance>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.Lances)
                .HasForeignKey(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}

