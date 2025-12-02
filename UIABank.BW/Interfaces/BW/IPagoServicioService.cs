using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public class CrearPagoServicioDto
    {
        public int ClienteId { get; set; }
        public int ProveedorServicioId { get; set; }
        public string NumeroContrato { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
        public string CuentaOrigen { get; set; }
        public DateTime? FechaProgramada { get; set; }
    }

    public class PagoServicioResultadoDto
    {
        public int PagoServicioId { get; set; }
        public string NumeroReferencia { get; set; }
        public EstadoPagoServicio Estado { get; set; }
    }

    public interface IPagoServicioService
    {
        Task<PagoServicioResultadoDto> CrearPagoAsync(CrearPagoServicioDto dto);

        Task<List<PagoServicio>> ObtenerPagosClienteAsync(
            int clienteId,
            DateTime? desde,
            DateTime? hasta,
            bool soloProgramados);

        Task<List<PagoServicio>> ObtenerTodosPagosAsync(
            DateTime? desde,
            DateTime? hasta,
            bool soloProgramados);

        Task CancelarPagoProgramadoAsync(int pagoServicioId, int clienteId);

        Task<byte[]> GenerarComprobantePdfAsync(int pagoServicioId);
    }
}
