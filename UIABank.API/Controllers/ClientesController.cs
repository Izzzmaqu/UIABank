using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteDto dto)
        {
            try
            {
                var cliente = await _clienteService.CrearClienteAsync(dto);
                return CreatedAtAction(
                    nameof(ObtenerCliente),
                    new { id = cliente.Id },
                    cliente);
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> ObtenerCliente(int id)
        {
            try
            {
                var cliente = await _clienteService.ObtenerClientePorIdAsync(id);
                return Ok(cliente);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> ObtenerTodosClientes()
        {
            var clientes = await _clienteService.ObtenerTodosClientesAsync();
            return Ok(clientes);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ActualizarClienteDto dto)
        {
            try
            {
                dto.Id = id;
                var cliente = await _clienteService.ActualizarClienteAsync(dto);
                return Ok(cliente);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}

