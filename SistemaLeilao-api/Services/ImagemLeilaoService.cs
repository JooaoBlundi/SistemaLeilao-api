using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using SistemaLeilao_api.Data;
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_api.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Services
{
    public class ImagemLeilaoService : Notifiable<Notification>, IImagemLeilaoService
    {
        private readonly LeilaoDbContext _context;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB limit per image
        private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };

        public ImagemLeilaoService(LeilaoDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, List<ImagemLeilao>? Data, List<Notification> Errors)> AddImagensAsync(long leilaoId, List<IFormFile> files)
        {
            var savedImages = new List<ImagemLeilao>();
            var errors = new List<Notification>();

            // Check if leilao exists (redundant if checked in controller, but good practice)
            var leilaoExists = await _context.Leiloes.AnyAsync(l => l.Id == leilaoId);
            if (!leilaoExists)
            {
                errors.Add(new Notification("LeilaoId", "Leilão não encontrado."));
                return (false, null, errors);
            }

            foreach (var file in files)
            {
                // Validate file size
                if (file.Length == 0 || file.Length > _maxFileSize)
                {
                    errors.Add(new Notification(file.FileName, $"Arquivo muito grande (máx. {_maxFileSize / 1024 / 1024} MB)."));
                    continue; // Skip this file
                }

                // Validate file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                {
                    errors.Add(new Notification(file.FileName, "Tipo de arquivo inválido. Permitidos: " + string.Join(", ", _allowedExtensions)));
                    continue; // Skip this file
                }

                // Read file content
                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                // Determine if this is the first image for the auction to set as principal
                bool isPrincipal = !await _context.ImagensLeilao.AnyAsync(img => img.LeilaoId == leilaoId);

                var imagemLeilao = new ImagemLeilao
                {
                    LeilaoId = leilaoId,
                    DadosImagem = fileData,
                    ContentType = file.ContentType,
                    NomeArquivo = file.FileName,
                    IsPrincipal = isPrincipal, // Set first uploaded image as principal by default
                    DataUpload = DateTime.UtcNow
                };

                _context.ImagensLeilao.Add(imagemLeilao);
                savedImages.Add(imagemLeilao); // Add to list before saving changes
            }

            if (errors.Any() && !savedImages.Any())
            {
                // Only errors, no images saved
                return (false, null, errors);
            }

            await _context.SaveChangesAsync();

            // If there were errors but some images were saved, return success=true but include errors
            return (true, savedImages, errors);
        }

        public async Task<IEnumerable<ImagemLeilao>> GetImagensByLeilaoIdAsync(long leilaoId)
        {
            // Return only necessary fields (e.g., Id, IsPrincipal, ContentType) to avoid sending large byte arrays
            // Or create a DTO
            return await _context.ImagensLeilao
                                 .Where(img => img.LeilaoId == leilaoId)
                                 .OrderByDescending(img => img.IsPrincipal) // Show principal first
                                 .ThenBy(img => img.DataUpload)
                                 .Select(img => new ImagemLeilao { // Project to avoid sending DadosImagem
                                     Id = img.Id,
                                     LeilaoId = img.LeilaoId,
                                     ContentType = img.ContentType,
                                     NomeArquivo = img.NomeArquivo,
                                     IsPrincipal = img.IsPrincipal,
                                     DataUpload = img.DataUpload
                                 })
                                 .ToListAsync();
        }

        public async Task<ImagemLeilao?> GetImagemByIdAsync(long imagemId)
        {
            // This method needs to return the full image data
            return await _context.ImagensLeilao.FindAsync(imagemId);
        }

        public async Task<bool> SetImagemPrincipalAsync(long leilaoId, long imagemId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Unset current principal image for the auction
                var currentPrincipal = await _context.ImagensLeilao
                                                     .FirstOrDefaultAsync(img => img.LeilaoId == leilaoId && img.IsPrincipal);
                if (currentPrincipal != null)
                {
                    currentPrincipal.IsPrincipal = false;
                }

                // Set the new principal image
                var newPrincipal = await _context.ImagensLeilao
                                                 .FirstOrDefaultAsync(img => img.Id == imagemId && img.LeilaoId == leilaoId);

                if (newPrincipal == null)
                {
                    await transaction.RollbackAsync();
                    AddNotification("ImagemId", "Imagem não encontrada ou não pertence ao leilão.");
                    return false;
                }

                newPrincipal.IsPrincipal = true;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log exception
                AddNotification("Database", "Erro ao definir imagem principal: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteImagemAsync(long imagemId)
        {
            var imagem = await _context.ImagensLeilao.FindAsync(imagemId);
            if (imagem == null)
            {
                AddNotification("ImagemId", "Imagem não encontrada.");
                return false;
            }

            // Prevent deleting the last image or the principal image? (Business rule)
            // Example: Check if it's the principal and if there are other images
            if (imagem.IsPrincipal)
            {
                 var otherImagesExist = await _context.ImagensLeilao.AnyAsync(img => img.LeilaoId == imagem.LeilaoId && img.Id != imagemId);
                 if (otherImagesExist)
                 {
                     AddNotification("ImagemPrincipal", "Não é possível excluir a imagem principal. Defina outra imagem como principal primeiro.");
                     return false;
                 }
                 // Optional: Allow deleting if it's the only image left?
            }

            _context.ImagensLeilao.Remove(imagem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

