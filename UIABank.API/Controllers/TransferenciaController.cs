using Microsoft.AspNetCore.Mvc;
using UIABank.BC.Modelos;
using UIABank.BW.CU;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferenciaController : ControllerBase
    {
        private readonly ITransferenciaBW transferenciaBW;

        public TransferenciaController(ITransferenciaBW transferenciaBW)
        {
            this.transferenciaBW = transferenciaBW;
        }

        // RF-D1: Crear transferencia (Pre-check + ejecución)

        [HttpPost("crear")]
        public async Task<IActionResult> CrearTransferencia([FromBody] Transferencia transferencia)
        {
            if (transferencia == null)
                return BadRequest("Los datos de la transferencia son inválidos.");

            var resultado = await transferenciaBW.CrearAsync(transferencia);

            if (!resultado)
                return BadRequest("No se pudo procesar la transferencia. Revise las validaciones.");

            return Ok("Transferencia creada correctamente.");
        }


        // RF-D2: Actualizar estado (Pendiente, Exitosa, Fallida, etc.)

        [HttpPut("actualizar-estado/{id}")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromQuery] EstadoTransferencia nuevoEstado)
        {
            var resultado = await transferenciaBW.ActualizarEstadoAsync(id, nuevoEstado);

            if (!resultado)
                return BadRequest("No se pudo actualizar el estado. Verifique el ID o el estado ingresado.");

            return Ok("Estado de la transferencia actualizado correctamente.");
        }


        // RF-D4: Obtener transferencia por ID

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var transferencia = await transferenciaBW.ObtenerPorIdAsync(id);

            if (transferencia == null)
                return NotFound($"No se encontró la transferencia con ID {id}.");

            return Ok(transferencia);
        }


        // RF-D4: Listar transferencias por usuario

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
        {
            var lista = await transferenciaBW.ListarPorUsuarioAsync(usuarioId);

            return Ok(lista);


        }
        [HttpPost("ejecutar")]
        public async Task<IActionResult> Ejecutar(
    [FromBody] Transferencia transferencia,
    [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
        {
            if (transferencia == null)
                return BadRequest("Los datos de la transferencia son inválidos."); 

          
            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                transferencia.IdempotencyKey = idempotencyKey;
            }
            else if (string.IsNullOrWhiteSpace(transferencia.IdempotencyKey))
            {
                transferencia.IdempotencyKey = Guid.NewGuid().ToString();
            }

            var ok = await transferenciaBW.EjecutarAsync(transferencia);
            if (!ok)
                return BadRequest("No se pudo ejecutar la transferencia.");

            return Ok("Transferencia ejecutada correctamente.");
        }

    }





    }
