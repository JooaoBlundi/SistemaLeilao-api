using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaLeilao_api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult PublicHealthCheck()
        {
            return Ok(new { status = "Healthy", message = "API is running." });
        }
       
    }
}
