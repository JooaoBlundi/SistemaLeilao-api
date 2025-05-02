using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for bid-related actions
    public class LancesController : ControllerBase
    {
        private readonly ILanceService _lanceService;

        public LancesController(ILanceService lanceService)
        {
            _lanceService = lanceService;
        }

        // POST /api/lances
        [HttpPost]
        public async Task<IActionResult> PlaceBid([FromBody] PlaceBidDto bidDto) // Assuming PlaceBidDto exists or will be created
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the authenticated user's ID (bidder)
            var compradorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(compradorIdString, out long compradorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var newLance = await _lanceService.PlaceBidAsync(bidDto.LeilaoId, bidDto.Valor, compradorId);

            if (newLance == null)
            {
                // Check notifications from the service
                if (_lanceService is Services.LanceService lanceServiceInstance && !lanceServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = lanceServiceInstance.Notifications });
                }
                // Generic error
                return BadRequest(new { message = "Falha ao registrar lance." });
            }

            // Return the created bid (or just a success message)
            // Consider creating a LanceDto to return
            return Ok(newLance); // Returning the Lance entity for now
        }

        // GET /api/lances/leilao/{leilaoId}
        [HttpGet("leilao/{leilaoId}")]
        [AllowAnonymous] // Allow anyone to see bids for an auction
        public async Task<IActionResult> GetBidsByLeilao(long leilaoId)
        {
            var lances = await _lanceService.GetBidsByLeilaoAsync(leilaoId);
            // Consider mapping to DTOs
            return Ok(lances);
        }

        // GET /api/lances/meus
        [HttpGet("meus")]
        public async Task<IActionResult> GetMyBids()
        {
            // Get the authenticated user's ID (bidder)
            var compradorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(compradorIdString, out long compradorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var lances = await _lanceService.GetBidsByUserAsync(compradorId);
            // Consider mapping to DTOs
            return Ok(lances);
        }
    }

    // DTO for placing a bid (needs to be created in DTOs folder)
    public class PlaceBidDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public long LeilaoId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.Range(0.01, double.MaxValue, ErrorMessage = "O valor do lance deve ser positivo.")]
        public decimal Valor { get; set; }
    }
}

