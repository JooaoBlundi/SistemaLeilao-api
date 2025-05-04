using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.Models;
using SistemaLeilao_api.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic; 

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/leilao")]
    [Authorize] 
    public class LeiloesController : ControllerBase
    {
        private readonly ILeilaoService _leilaoService;

        public LeiloesController(ILeilaoService leilaoService)
        {
            _leilaoService = leilaoService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeilao([FromBody] CreateLeilaoDto leilaoDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            
            var vendedorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(vendedorIdString, out long vendedorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var newLeilao = await _leilaoService.CreateLeilaoAsync(leilaoDto, vendedorId);

            if (newLeilao == null)
            {
                if (_leilaoService is Services.LeilaoService leilaoServiceInstance && !leilaoServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = leilaoServiceInstance.Notifications });
                }
                return BadRequest(new { message = "Falha ao criar leilão." });
            }

            return CreatedAtAction(nameof(GetLeilaoById), new { id = newLeilao.Id }, newLeilao); 
        }

        [HttpGet("ativos")]
        public async Task<IActionResult> GetActiveLeiloes()
        {
            var leiloes = await _leilaoService.GetActiveLeiloesAsync();
            return Ok(leiloes);
        }

        [HttpGet("meus")]
        public async Task<IActionResult> GetUserLeiloes()
        {
            var vendedorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(vendedorIdString, out long vendedorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var leiloes = await _leilaoService.GetUserLeiloesAsync(vendedorId);
            return Ok(leiloes);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetLeilaoById(long id)
        {
            await Task.Delay(10);
            return Ok(new { message = $"Endpoint GetLeilaoById({id}) placeholder." });
        }

    }
}

