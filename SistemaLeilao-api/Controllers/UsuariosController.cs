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
                // Check notifications from the service
                if (_userService is Services.UserService userServiceInstance && !userServiceInstance.IsValid)
                {
                    return BadRequest(new { errors = userServiceInstance.Notifications });
                }
                // Generic error if registration failed for other reasons (should have notifications)
                return BadRequest(new { message = "Falha ao registrar usuário." }); 
            }

            // Return the created user (or just a success message)
            // Avoid returning the password hash
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, new { message = "Usuário registrado com sucesso!" /*, user = newUser */ }); // Consider creating a UserDto to return
        }

        // Example placeholder for getting a user by ID (requires authentication)
        [HttpGet("{id}")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> GetUserById(long id)
        {
            // TODO: Implement GetUserById in IUserService and UserService
            // var user = await _userService.GetUserByIdAsync(id);
            // if (user == null) return NotFound();
            // return Ok(user); // Return a UserDto, not the entity with password hash
            await Task.Delay(10); // Placeholder async operation
            return Ok(new { message = $"Endpoint GetUserById({id}) placeholder." });
        }
    }
}

