using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ProveedoresServiciosController : ControllerBase
    {
        private readonly IProveedorServicioService _service;

        public ProveedoresServiciosController(IProveedorServicioService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CrearProveedor([FromBody] CrearProveedorServicioDto dto)
        {
            try
            {
                var proveedor = await _service.CrearProveedorAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = proveedor.Id }, proveedor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProveedor(int id, [FromBody] ActualizarProveedorServicioDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { error = "El id de la ruta no coincide con el del cuerpo" });

            try
            {
                var proveedor = await _service.ActualizarProveedorAsync(dto);
                return Ok(proveedor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous] // o solo Admin/Gestor, según quieras
        public async Task<IActionResult> ObtenerTodos()
        {
            var proveedores = await _service.ObtenerTodosAsync();
            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var proveedor = await _service.ObtenerPorIdAsync(id);
            if (proveedor == null) return NotFound();
            return Ok(proveedor);
        }
    }
}
