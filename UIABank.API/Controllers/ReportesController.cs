using Microsoft.AspNetCore.Mvc;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        [HttpGet("totales")]
        public async Task<IActionResult> GetTotales(DateTime desde, DateTime hasta)
        {
            return Ok(await _reportesService.ObtenerTotalesAsync(desde, hasta));
        }

        [HttpGet("top10")]
        public async Task<IActionResult> GetTop10(DateTime desde, DateTime hasta)
        {
            return Ok(await _reportesService.ObtenerTop10ClientesAsync(desde, hasta));
        }

        [HttpGet("volumen-diario")]
        public async Task<IActionResult> GetVolumenDiario(DateTime desde, DateTime hasta)
        {
            return Ok(await _reportesService.ObtenerVolumenDiarioAsync(desde, hasta));
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GetPdf(DateTime desde, DateTime hasta)
        {
            var pdf = await _reportesService.GenerarReportePdfAsync(desde, hasta);
            return File(pdf, "application/pdf", "reporte.pdf");
        }

        [HttpGet("excel")]
        public async Task<IActionResult> GetExcel(DateTime desde, DateTime hasta)
        {
            var file = await _reportesService.GenerarReporteExcelAsync(desde, hasta);
            return File(file, "text/csv", "reporte.csv");
        }
    }
}

