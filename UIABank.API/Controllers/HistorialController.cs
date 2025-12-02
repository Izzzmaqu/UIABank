using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistorialController : ControllerBase
    {
        private readonly IHistorialService _historialService;

        public HistorialController(IHistorialService historialService)
        {
            _historialService = historialService;
        }

        //RF-F1: Historial cliente
      

        [HttpGet]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ObtenerHistorialCliente(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] string? tipo,
            [FromQuery] string? estado)
        {
            var clienteId = ObtenerClienteIdDesdeClaims();

            TipoMovimiento? tipoMovimiento = null;
            if (!string.IsNullOrWhiteSpace(tipo))
            {
               
                if (tipo.Equals("transferencia", StringComparison.OrdinalIgnoreCase))
                    tipoMovimiento = TipoMovimiento.Transferencia;
                else if (tipo.Equals("pago", StringComparison.OrdinalIgnoreCase) ||
                         tipo.Equals("pagoservicio", StringComparison.OrdinalIgnoreCase))
                    tipoMovimiento = TipoMovimiento.PagoServicio;
                else if (Enum.TryParse<TipoMovimiento>(tipo, true, out var parsed))
                    tipoMovimiento = parsed;
            }

            var movimientos = await _historialService.ObtenerHistorialClienteAsync(
                clienteId,
                desde,
                hasta,
                tipoMovimiento,
                estado);

            return Ok(movimientos);
        }

        // RF-F1: Historial admin/gestor 
       

        [HttpGet("admin")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> ObtenerHistorialAdmin(
            [FromQuery] int clienteId,
            [FromQuery] Guid? cuentaId,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] string? tipo,
            [FromQuery] string? estado)
        {
            TipoMovimiento? tipoMovimiento = null;
            if (!string.IsNullOrWhiteSpace(tipo))
            {
                if (tipo.Equals("transferencia", StringComparison.OrdinalIgnoreCase))
                    tipoMovimiento = TipoMovimiento.Transferencia;
                else if (tipo.Equals("pago", StringComparison.OrdinalIgnoreCase) ||
                         tipo.Equals("pagoservicio", StringComparison.OrdinalIgnoreCase))
                    tipoMovimiento = TipoMovimiento.PagoServicio;
                else if (Enum.TryParse<TipoMovimiento>(tipo, true, out var parsed))
                    tipoMovimiento = parsed;
            }

            var movimientos = await _historialService.ObtenerHistorialPorClienteCuentaAsync(
                clienteId,
                cuentaId,
                desde,
                hasta,
                tipoMovimiento,
                estado);

            return Ok(movimientos);
        }

        // RF-F2: Extractos 
     
        [HttpGet("extracto")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> GenerarExtracto(
            [FromQuery] Guid cuentaId,
            [FromQuery] int anio,
            [FromQuery] int mes,
            [FromQuery] string formato = "pdf")
        {
            var clienteId = ObtenerClienteIdDesdeClaims();
            formato = formato?.ToLowerInvariant() ?? "pdf";

            if (formato == "csv")
            {
                var bytes = await _historialService.GenerarExtractoMensualCsvAsync(
                    clienteId, cuentaId, anio, mes);
                var fileName = $"extracto_{anio}_{mes:D2}.csv";
                return File(bytes, "text/csv", fileName);
            }
            else
            {
                var bytes = await _historialService.GenerarExtractoMensualPdfAsync(
                    clienteId, cuentaId, anio, mes);
                var fileName = $"extracto_{anio}_{mes:D2}.pdf";
                return File(bytes, "application/pdf", fileName);
            }
        }

        //  RF-F3: Comprobantes 

      
        [HttpGet("comprobantes/transferencia/{id}")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ComprobanteTransferencia(int id)
        {
            var clienteId = ObtenerClienteIdDesdeClaims();
            var bytes = await _historialService.GenerarComprobanteTransferenciaPdfAsync(id, clienteId);
            var fileName = $"comprobante_transferencia_{id}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

     
        [HttpGet("comprobantes/pago/{id}")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ComprobantePago(int id)
        {
            var clienteId = ObtenerClienteIdDesdeClaims();
            var bytes = await _historialService.GenerarComprobantePagoServicioPdfAsync(id, clienteId);
            var fileName = $"comprobante_pago_{id}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

       

        private int ObtenerClienteIdDesdeClaims()
        {
            var claim = User.FindFirst("ClienteId");
            if (claim == null)
                throw new InvalidOperationException("El token no contiene el ClienteId.");

            if (!int.TryParse(claim.Value, out var clienteId))
                throw new InvalidOperationException("El ClienteId del token no es válido.");

            return clienteId;
        }
    }
}
