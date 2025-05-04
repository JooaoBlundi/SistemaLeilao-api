using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.Models;
using SistemaLeilao_api.Interfaces;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous] 
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.AuthenticateAsync(model);

            if (token == null)
            {
                if (_authService is Services.AuthService authServiceInstance && !authServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = authServiceInstance.Notifications });
                }
                return Unauthorized(new { message = "Usuário ou senha inválidos." }); 
            }

            return Ok(new { token = token });
        }

    }
}

