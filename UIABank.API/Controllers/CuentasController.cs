using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UIABank.BW.CU;
using UIABank.BW.Cuentas;
using UIABank.BW.Interfaces.BW;
using UIABank.BC.Modelos;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;

        public CuentasController(ICuentaService cuentaService)
        {
            _cuentaService = cuentaService;
        }

        // POST: api/cuentas
        // Solo Admin o Gestor pueden abrir cuentas (RF-B1)
        [HttpPost]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<CuentaDto>> AbrirCuenta([FromBody] AbrirCuentaRequest request)
        {
            var cuenta = await _cuentaService.AbrirCuentaAsync(request);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cuenta.Id }, cuenta);
        }

        // GET: api/cuentas/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<CuentaDto>> ObtenerPorId(Guid id)
        {
            var cuenta = await _cuentaService.ObtenerPorIdAsync(id);
            if (cuenta == null)
                return NotFound();

            return Ok(cuenta);
        }

        // GET: api/cuentas/por-cliente/{clienteId}
        // Admin / Gestor consultan por cliente (RF-B2)
        [HttpGet("por-cliente/{clienteId:guid}")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<IReadOnlyList<CuentaDto>>> ObtenerPorCliente(Guid clienteId)
        {
            var cuentas = await _cuentaService.ObtenerCuentasPorClienteAsync(clienteId);
            return Ok(cuentas);
        }

        // GET: api/cuentas?clienteId=...&tipo=...&moneda=...&estado=...
        // Filtros para admin/gestor (RF-B2)
        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<ActionResult<IReadOnlyList<CuentaDto>>> Buscar(
            [FromQuery] Guid? clienteId,
            [FromQuery] int? tipo,
            [FromQuery] int? moneda,
            [FromQuery] int? estado)
        {
            var filtro = new CuentasFiltroRequest
            {
                ClienteId = clienteId,
                Tipo = tipo.HasValue ? (TipoCuenta?)tipo.Value : null,
                Moneda = moneda.HasValue ? (Moneda?)moneda.Value : null,
                Estado = estado.HasValue ? (EstadoCuenta?)estado.Value : null
            };

            var cuentas = await _cuentaService.BuscarCuentasAsync(filtro);
            return Ok(cuentas);
        }

        // PUT: api/cuentas/{id}/bloquear
        [HttpPut("{id:guid}/bloquear")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Bloquear(Guid id)
        {
            await _cuentaService.BloquearCuentaAsync(id);
            return NoContent();
        }

        // PUT: api/cuentas/{id}/cerrar
        [HttpPut("{id:guid}/cerrar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Cerrar(Guid id)
        {
            await _cuentaService.CerrarCuentaAsync(id);
            return NoContent();
        }
    }
}