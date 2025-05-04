using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_web.DTOs;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsuariosController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add")]
        [AllowAnonymous] 
        public async Task<ActionResult> AddUsuario([FromBody] UsuarioModel registerDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            var newUser = await _userService.RegisterUserAsync(registerDto);

            if (newUser == null)
            {
                if (_userService is Services.UserService userServiceInstance && !userServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = userServiceInstance.Notifications });
                }
                return BadRequest(new { message = "Falha ao registrar usuário." }); 
            }

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, new { message = "Usuário registrado com sucesso!" /*, user = newUser */ }); // Consider creating a UserDto to return
        }

        [HttpGet("{id}")]
        [Authorize] 
        public async Task<IActionResult> GetUserById(long id)
        {
            await Task.Delay(10);
            return Ok(new { message = $"Endpoint GetUserById({id}) placeholder." });
        }
    }
}

