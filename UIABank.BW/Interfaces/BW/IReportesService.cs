using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos.Reportes;

namespace UIABank.BW.Interfaces.BW
{
    public interface IReportesService
    {
        Task<ReporteTotalesDto> ObtenerTotalesAsync(DateTime desde, DateTime hasta);
        Task<List<ClienteVolumenDto>> ObtenerTop10ClientesAsync(DateTime desde, DateTime hasta);
        Task<List<VolumenDiarioDto>> ObtenerVolumenDiarioAsync(DateTime desde, DateTime hasta);

        Task<byte[]> GenerarReportePdfAsync(DateTime desde, DateTime hasta);
        Task<byte[]> GenerarReporteExcelAsync(DateTime desde, DateTime hasta);
    }
}
