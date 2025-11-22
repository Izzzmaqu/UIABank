using Microsoft.AspNetCore.Mvc;
using UIABank.BC.Modelos;
using UIABank.BW.CU;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciaProgramadaController : ControllerBase
    {
        private readonly ITransferenciaProgramadaBW programacionBW;

        public TransferenciaProgramadaController(ITransferenciaProgramadaBW programacionBW)
        {
            this.programacionBW = programacionBW;
        }

        [HttpPost("programar")]
        public async Task<IActionResult> Programar([FromBody] Transferencia transferencia)
        {
            if (transferencia.FechaProgramada == null)
                return BadRequest("Debe enviar fecha programada.");

            var ok = await programacionBW.CrearProgramada(transferencia);

            if (!ok)
                return BadRequest("No se pudo programar la transferencia.");

            return Ok("Transferencia programada correctamente.");
        }
        

        [HttpPut("programadas/{id}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var ok = await programacionBW.CancelarProgramadaAsync(id);
            if (!ok)
                return BadRequest("No se pudo cancelar la transferencia programada.");

            return Ok("Transferencia programada cancelada correctamente.");
        }



    }
}
