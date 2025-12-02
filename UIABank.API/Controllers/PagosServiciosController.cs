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
    [Authorize(Roles = "Administrador,Cliente,Gestor")]
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

        [HttpPost]
        public async Task<IActionResult> CrearPago([FromBody] CrearPagoServicioDto dto)
        {
            try
            {
                var rol = User.FindFirst(ClaimTypes.Role)?.Value;

                if (rol == "Cliente")
                {
                    dto.ClienteId = ObtenerClienteIdDesdeClaims();
                }
                else if (rol == "Administrador" || rol == "Gestor")
                {
                    if (dto.ClienteId == 0)
                    {
                        dto.ClienteId = 1;
                    }
                }
                else
                {
                    return Unauthorized(new { error = "Rol no autorizado para realizar pagos" });
                }

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

        [HttpGet]
        public async Task<IActionResult> ObtenerPagos(
            [FromQuery] int? clienteId,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] bool soloProgramados = false)
        {
            try
            {
                var rol = User.FindFirst(ClaimTypes.Role)?.Value;

                int clienteIdFinal;

                if (rol == "Administrador" || rol == "Gestor")
                {
                    if (clienteId.HasValue)
                    {
                        clienteIdFinal = clienteId.Value;
                    }
                    else
                    {
                        var todosPagos = await _service.ObtenerTodosPagosAsync(desde, hasta, soloProgramados);
                        return Ok(todosPagos);
                    }
                }
                else
                {
                    clienteIdFinal = ObtenerClienteIdDesdeClaims();
                }

                var pagos = await _service.ObtenerPagosClienteAsync(clienteIdFinal, desde, hasta, soloProgramados);
                return Ok(pagos);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/comprobante")]
        public async Task<IActionResult> DescargarComprobante(int id)
        {
            try
            {
                var pdfBytes = await _service.GenerarComprobantePdfAsync(id);

                return File(
                    fileContents: pdfBytes,
                    contentType: "application/pdf",
                    fileDownloadName: $"Comprobante_Pago_{id}.pdf"
                );
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Error al generar comprobante" });
            }
        }
    }
}
