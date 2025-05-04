using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.Interfaces;
using System.Security.Claims;

namespace SistemaLeilao_api.Controllers
{
    [Route("api/imagem")]
    [ApiController]
    [Authorize] 
    public class ImagensController : ControllerBase
    {
        private readonly IImagemLeilaoService _imagemService;
        private readonly ILeilaoService _leilaoService; 

        public ImagensController(IImagemLeilaoService imagemService, ILeilaoService leilaoService)
        {
            _imagemService = imagemService;
            _leilaoService = leilaoService;
        }

        [HttpPost("Upload/{leilaoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadImagens(long leilaoId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Nenhuma imagem foi enviada.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long numericUserId))
            {
                return Unauthorized("Usuário não autenticado ou ID inválido.");
            }

            var leilao = await _leilaoService.GetLeilaoByIdAsync(leilaoId); 
            if (leilao == null)
            {
                return NotFound("Leilão não encontrado.");
            }
            if (leilao.VendedorId != numericUserId)
            {
                return Forbid("Você não tem permissão para adicionar imagens a este leilão.");
            }

            var result = await _imagemService.AddImagensAsync(leilaoId, files);

            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }

        [HttpGet("Leilao/{leilaoId}")]
        [AllowAnonymous] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImagensPorLeilao(long leilaoId)
        {
            var imagens = await _imagemService.GetImagensByLeilaoIdAsync(leilaoId);
            if (imagens == null || !imagens.Any())
            {
                return NotFound("Nenhuma imagem encontrada para este leilão.");
            }
            return Ok(imagens);
        }

        [HttpGet("{imagemId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImagem(long imagemId)
        {
            var imagem = await _imagemService.GetImagemByIdAsync(imagemId);
            if (imagem == null || imagem.DadosImagem == null || string.IsNullOrEmpty(imagem.ContentType))
            {
                return NotFound();
            }
            return File(imagem.DadosImagem, imagem.ContentType, imagem.NomeArquivo ?? $"imagem_{imagemId}");
        }

        [HttpPost("SetPrincipal/{leilaoId}/{imagemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetImagemPrincipal(long leilaoId, long imagemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long numericUserId))
            {
                return Unauthorized("Usuário não autenticado ou ID inválido.");
            }

            var leilao = await _leilaoService.GetLeilaoByIdAsync(leilaoId);
            if (leilao == null)
            {
                return NotFound("Leilão não encontrado.");
            }
            if (leilao.VendedorId != numericUserId)
            {
                return Forbid("Você não tem permissão para modificar este leilão.");
            }

            var success = await _imagemService.SetImagemPrincipalAsync(leilaoId, imagemId);

            if (!success)
            {
                return NotFound("Imagem não encontrada ou não pertence ao leilão especificado.");
            }

            return NoContent();
        }

        [HttpDelete("{imagemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImagem(long imagemId)
        {
             var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long numericUserId))
            {
                return Unauthorized("Usuário não autenticado ou ID inválido.");
            }

            var imagem = await _imagemService.GetImagemByIdAsync(imagemId);
            if (imagem == null)
            {
                return NotFound("Imagem não encontrada.");
            }

            var leilao = await _leilaoService.GetLeilaoByIdAsync(imagem.LeilaoId);
             if (leilao == null || leilao.VendedorId != numericUserId)
            {
                return Forbid("Você não tem permissão para excluir esta imagem.");
            }

            var success = await _imagemService.DeleteImagemAsync(imagemId);
            if (!success)
            {
                return NotFound(); 
            }

            return NoContent();
        }

    }
}

