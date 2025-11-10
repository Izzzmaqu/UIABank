using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.DA
{
    public interface ITransferenciaDA
    {
        // Guarda una nueva transferencia.
        Task<bool> CrearAsync(Transferencia transferencia);

        // Obtiene una transferencia por su ID.
        Task<Transferencia?> ObtenerPorIdAsync(int id);

        // Lista todas las transferencias de un usuario.
        Task<IEnumerable<Transferencia>> ListarPorUsuarioAsync(int usuarioId);

        // Actualiza el estado de una transferencia existente.
        Task<bool> ActualizarEstadoAsync(int id, EstadoTransferencia nuevoEstado);

        Task<Transferencia?> ObtenerPorIdempotencyKeyAsync(string key);

        Task<bool> EjecutarTransferenciaAsync(Transferencia transferencia, Cuenta cuentaOrigen, CuentaFake? cuentaDestino);

        Task<decimal> ObtenerTotalDiarioAsync(int usuarioId, DateTime fecha, string moneda);

    }
}
