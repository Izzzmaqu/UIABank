using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Cliente")]
    public class PagosServiciosController : ControllerBase
    {
        private readonly IPagoServicioService _service;

        public PagosServiciosController(IPagoServicioService service)
        {
            _service = service;
        }

        private int ObtenerClienteIdDesdeClaims()
        {
            var claim = User.FindFirst("ClienteId");
            if (claim == null)
                throw new InvalidOperationException("El token no contiene el ClienteId");

            return int.Parse(claim.Value);
        }

        // RF-E2 y RF-E3: pago inmediato o programado, depende de FechaProgramada
        [HttpPost]
        public async Task<IActionResult> CrearPago([FromBody] CrearPagoServicioDto dto)
        {
            try
            {
                var clienteId = ObtenerClienteIdDesdeClaims();
                dto.ClienteId = clienteId;

                var resultado = await _service.CrearPagoAsync(dto);
                return Ok(resultado);
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

        // RF-E3: cancelar pagos programados hasta 24h antes
        [HttpDelete("{id}/cancelar")]
        public async Task<IActionResult> CancelarPagoProgramado(int id)
        {
            try
            {
                var clienteId = ObtenerClienteIdDesdeClaims();
                await _service.CancelarPagoProgramadoAsync(id, clienteId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Para ver el historial de pagos de servicios del cliente (ayuda también a Módulo F)
        [HttpGet]
        public async Task<IActionResult> ObtenerPagos(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] bool soloProgramados = false)
        {
            var clienteId = ObtenerClienteIdDesdeClaims();
            var pagos = await _service.ObtenerPagosClienteAsync(clienteId, desde, hasta, soloProgramados);
            return Ok(pagos);
        }
    }
}
