using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.DTOs;
using SistemaLeilao_api.Interfaces;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous] // Allow access without authentication
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) // Basic model validation
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.AuthenticateAsync(loginDto);

            if (token == null)
            {
                // Check notifications from the service
                if (_authService is Services.AuthService authServiceInstance && !authServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = authServiceInstance.Notifications });
                }
                // Generic unauthorized if no specific notification (shouldn't happen with current logic but good practice)
                return Unauthorized(new { message = "Usuário ou senha inválidos." }); 
            }

            return Ok(new { token = token });
        }

        // TODO: Add a registration endpoint later, likely in a UserController
        // TODO: Add endpoints for token refresh if needed
    }
}

