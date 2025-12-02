using Microsoft.AspNetCore.Mvc;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            var eventos = await _auditoriaService.ObtenerEventosAsync(desde, hasta);
            return Ok(eventos);
        }
    }
}
