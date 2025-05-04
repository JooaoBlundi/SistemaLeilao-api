using Microsoft.EntityFrameworkCore;
using SistemaLeilao_api.Entities;

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
        public DbSet<ImagemLeilao> ImagensLeilao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Cpf)
                .IsUnique();

            modelBuilder.Entity<Leilao>()
                .HasOne(l => l.Vendedor)
                .WithMany(u => u.LeiloesVendidos)
                .HasForeignKey(l => l.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Leilao>()
               .HasOne(l => l.Comprador)
               .WithMany(u => u.LeiloesComprados)
               .HasForeignKey(l => l.CompradorId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lance>()
                .HasOne(l => l.Leilao)
                .WithMany(le => le.Lances)
                .HasForeignKey(l => l.LeilaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lance>()
               .HasOne(l => l.Usuario)
               .WithMany(u => u.Lances)
               .HasForeignKey(l => l.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ImagemLeilao>()
                .HasOne(img => img.Leilao)
                .WithMany(l => l.ImagensLeilao)
                .HasForeignKey(img => img.LeilaoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

