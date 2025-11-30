using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public interface IHistorialService
    {
        // RF-F1: Historial para cliente autenticado
        Task<List<MovimientoHistorial>> ObtenerHistorialClienteAsync(
            int clienteId,
            DateTime? desde,
            DateTime? hasta,
            TipoMovimiento? tipo,
            string? estado);

        // RF-F1: Historial para admin/gestor 
        Task<List<MovimientoHistorial>> ObtenerHistorialPorClienteCuentaAsync(
            int clienteId,
            Guid? cuentaId,
            DateTime? desde,
            DateTime? hasta,
            TipoMovimiento? tipo,
            string? estado);

        // RF-F2: Extracto mensual 
        Task<ExtractoMensual> GenerarExtractoMensualAsync(
            int clienteId,
            Guid cuentaId,
            int anio,
            int mes);

        // RF-F2: Extracto mensual CSV
        Task<byte[]> GenerarExtractoMensualCsvAsync(
            int clienteId,
            Guid cuentaId,
            int anio,
            int mes);

        // RF-F2: Extracto mensual "PDF" 
        Task<byte[]> GenerarExtractoMensualPdfAsync(
            int clienteId,
            Guid cuentaId,
            int anio,
            int mes);

        // RF-F3: Comprobantes en PDF 
        Task<byte[]> GenerarComprobanteTransferenciaPdfAsync(int transferenciaId, int clienteId);
        Task<byte[]> GenerarComprobantePagoServicioPdfAsync(int pagoServicioId, int clienteId);
    }
}

