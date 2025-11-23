using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UIABank.BW.CU;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Cliente")] // todos los endpoints son para clientes según el enunciado
    public class BeneficiariosController : ControllerBase
    {
        private readonly IBeneficiarioService _beneficiarioService;

        public BeneficiariosController(IBeneficiarioService beneficiarioService)
        {
            _beneficiarioService = beneficiarioService;
        }

        // POST: api/beneficiarios
        [HttpPost]
        public async Task<ActionResult<BeneficiarioDto>> Registrar([FromBody] RegistrarBeneficiarioRequest request)
        {
            // En una versión más avanzada, ClienteId debería venir del JWT, no del body.
            var beneficiario = await _beneficiarioService.RegistrarAsync(request);
            return CreatedAtAction(nameof(ObtenerPorId),
                new { id = beneficiario.Id, clienteId = beneficiario.ClienteId },
                beneficiario);
        }

        // POST: api/beneficiarios/{id}/confirmar
        [HttpPost("{id:guid}/confirmar")]
        public async Task<IActionResult> Confirmar(Guid id, [FromQuery] Guid clienteId)
        {
            await _beneficiarioService.ConfirmarAsync(id, clienteId);
            return NoContent();
        }

        // PUT: api/beneficiarios/{id}/alias
        [HttpPut("{id:guid}/alias")]
        public async Task<IActionResult> ActualizarAlias(Guid id, [FromBody] ActualizarAliasBeneficiarioRequest request)
        {
            await _beneficiarioService.ActualizarAliasAsync(id, request);
            return NoContent();
        }

        // DELETE: api/beneficiarios/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Eliminar(Guid id, [FromQuery] Guid clienteId)
        {
            await _beneficiarioService.EliminarAsync(id, clienteId);
            return NoContent();
        }

        // GET: api/beneficiarios/{id}?clienteId=...
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BeneficiarioDto>> ObtenerPorId(Guid id, [FromQuery] Guid clienteId)
        {
            var beneficiario = await _beneficiarioService.ObtenerPorIdAsync(id, clienteId);
            if (beneficiario == null)
                return NotFound();

            return Ok(beneficiario);
        }

        // GET: api/beneficiarios?clienteId=...&alias=...&banco=...&pais=...
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<BeneficiarioDto>>> ObtenerPorCliente(
            [FromQuery] Guid clienteId,
            [FromQuery] string? alias,
            [FromQuery] string? banco,
            [FromQuery] string? pais)
        {
            var filtro = new BeneficiariosFiltroRequest
            {
                ClienteId = clienteId,
                Alias = alias,
                Banco = banco,
                Pais = pais
            };

            var lista = await _beneficiarioService.ObtenerPorClienteAsync(filtro);
            return Ok(lista);
        }
    }
}