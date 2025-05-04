using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class LancesController : ControllerBase
    {
        private readonly ILanceService _lanceService;

        public LancesController(ILanceService lanceService)
        {
            _lanceService = lanceService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid([FromBody] PlaceBidDto bidDto) // Assuming PlaceBidDto exists or will be created
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var compradorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(compradorIdString, out long compradorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var newLance = await _lanceService.PlaceBidAsync(bidDto.LeilaoId, bidDto.Valor, compradorId);

            if (newLance == null)
            {
                if (_lanceService is Services.LanceService lanceServiceInstance && !lanceServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = lanceServiceInstance.Notifications });
                }
                // Generic error
                return BadRequest(new { message = "Falha ao registrar lance." });
            }

            return Ok(newLance); 
        }

        [HttpGet("leilao/{leilaoId}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetBidsByLeilao(long leilaoId)
        {
            var lances = await _lanceService.GetBidsByLeilaoAsync(leilaoId);
            return Ok(lances);
        }

        [HttpGet("meus")]
        public async Task<IActionResult> GetMyBids()
        {
            var compradorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(compradorIdString, out long compradorId))
            {
                return Unauthorized(new { message = "ID do usuário inválido no token." });
            }

            var lances = await _lanceService.GetBidsByUserAsync(compradorId);
            return Ok(lances);
        }
    }

    public class PlaceBidDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public long LeilaoId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.Range(0.01, double.MaxValue, ErrorMessage = "O valor do lance deve ser positivo.")]
        public decimal Valor { get; set; }
    }
}

