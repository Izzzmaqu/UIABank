using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.DA;
using UIABank.DA.Config;

using UIABank.BC.Cuentas;


namespace UIABank.DA.Acciones
{
    public class TransferenciaDA : ITransferenciaDA
    {
        private readonly TransferenciaContext _context;

        public TransferenciaDA(TransferenciaContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearAsync(Transferencia transferencia)
        {
            _context.Transferencias.Add(transferencia);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Transferencia?> ObtenerPorIdAsync(int id)
        {
            return await _context.Transferencias
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transferencia>> ListarPorUsuarioAsync(int usuarioId)
        {
            return await _context.Transferencias
                .Where(t => t.UsuarioEjecutorId == usuarioId)
                .ToListAsync();
        }

        public async Task<bool> ActualizarEstadoAsync(int id, EstadoTransferencia nuevoEstado)
        {
            var transferencia = await _context.Transferencias.FirstOrDefaultAsync(t => t.Id == id);
            if (transferencia == null)
                return false;

            transferencia.Estado = nuevoEstado;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Transferencia?> ObtenerPorIdempotencyKeyAsync(string key)
        {
            return await _context.Transferencias
                .FirstOrDefaultAsync(t => t.IdempotencyKey == key);
        }

        //  RF-D2:

        public async Task<bool> EjecutarTransferenciaAsync(
            Transferencia transferencia,
            Cuenta cuentaOrigen,
            Cuenta? cuentaDestino)
        {
            using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                transferencia.FechaEjecucion = DateTime.Now;

                var existente = await _context.Transferencias
                    .FirstOrDefaultAsync(t => t.Id == transferencia.Id);

                if (existente == null)
                {
                    await _context.Transferencias.AddAsync(transferencia);
                }
                else
                {
                    existente.Estado = transferencia.Estado;
                    existente.FechaEjecucion = transferencia.FechaEjecucion;
                    existente.Monto = transferencia.Monto;
                    existente.Comision = transferencia.Comision;
                    existente.CuentaOrigenId = transferencia.CuentaOrigenId;
                    existente.CuentaDestinoId = transferencia.CuentaDestinoId;
                    existente.TerceroId = transferencia.TerceroId;
                    existente.Moneda = transferencia.Moneda;
                    existente.UsuarioEjecutorId = transferencia.UsuarioEjecutorId;
                    existente.IdempotencyKey = transferencia.IdempotencyKey;
                    existente.Referencia = transferencia.Referencia;
                }

            

                var cambios = await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                return cambios > 0;
            }
            catch
            {
                await transaccion.RollbackAsync();
                
                return false;
            }
        }


        async Task<decimal> ITransferenciaDA.ObtenerTotalDiarioAsync(int usuarioId, DateTime fecha, string moneda)
        {
            return await _context.Transferencias
       .Where(t =>
           t.UsuarioEjecutorId == usuarioId &&
           t.Moneda == moneda &&
           t.FechaCreacion.Date == fecha.Date &&
           (t.Estado == EstadoTransferencia.Exitosa ||
            t.Estado == EstadoTransferencia.PendienteAprobacion))
       .SumAsync(t => (decimal?)t.Monto) ?? 0m;
        }
    }
}