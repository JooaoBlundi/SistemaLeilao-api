using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.DTOs;
using SistemaLeilao_api.Interfaces;
using System.Security.Claims; // Required for getting user ID from claims
using System.Threading.Tasks;
using System.Collections.Generic; // Required for IEnumerable

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all actions in this controller by default
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
            if (!ModelState.IsValid) // Basic model validation
            {
                return BadRequest(ModelState);
            }

            // Get the authenticated user's ID from the token claims
            var vendedorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(vendedorIdString, out long vendedorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var newLeilao = await _leilaoService.CreateLeilaoAsync(leilaoDto, vendedorId);

            if (newLeilao == null)
            {
                // Check notifications from the service
                if (_leilaoService is Services.LeilaoService leilaoServiceInstance && !leilaoServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = leilaoServiceInstance.Notifications });
                }
                // Generic error
                return BadRequest(new { message = "Falha ao criar leilão." });
            }

            // Return the created auction (or just a success message)
            // Consider creating a LeilaoDto to return
            return CreatedAtAction(nameof(GetLeilaoById), new { id = newLeilao.Id }, newLeilao); 
        }

        // GET endpoint for active auctions (for dashboard)
        [HttpGet("ativos")]
        // [AllowAnonymous] // Keep Authorize based on dashboard image showing logged-in user
        public async Task<IActionResult> GetActiveLeiloes()
        {
            var leiloes = await _leilaoService.GetActiveLeiloesAsync();
            // Consider mapping to DTOs before returning
            return Ok(leiloes);
        }

        // GET endpoint for auctions created by the logged-in user
        [HttpGet("meus")]
        public async Task<IActionResult> GetUserLeiloes()
        {
             // Get the authenticated user's ID from the token claims
            var vendedorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(vendedorIdString, out long vendedorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var leiloes = await _leilaoService.GetUserLeiloesAsync(vendedorId);
            // Consider mapping to DTOs before returning
            return Ok(leiloes);
        }

        // Example placeholder for getting an auction by ID
        [HttpGet("{id}")]
        [AllowAnonymous] // Or [Authorize] depending on requirements
        public async Task<IActionResult> GetLeilaoById(long id)
        {
            // TODO: Implement GetLeilaoById in ILeilaoService and LeilaoService
            // var leilao = await _leilaoService.GetLeilaoByIdAsync(id);
            // if (leilao == null) return NotFound();
            // return Ok(leilao); // Return a LeilaoDto
            await Task.Delay(10); // Placeholder
            return Ok(new { message = $"Endpoint GetLeilaoById({id}) placeholder." });
        }

        // TODO: Add endpoints for placing bids, getting user bids (MeusLances), searching auctions (BuscarLances), etc.
    }
}

