using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.DA;
using UIABank.DA.Config;

namespace UIABank.DA.Acciones
{
    public class TransferenciaProgramadaDA : ITransferenciaProgramadaDA
    {
        private readonly UIABankDbContext _context;

        public TransferenciaProgramadaDA(UIABankDbContext context)
        {
            _context = context;
        }


        public async Task<bool> CrearAsync(ProgramacionTransferencia p)
        {
            _context.ProgramacionesTransferencias.Add(p);
            return await _context.SaveChangesAsync() > 0;
        }

      
        public async Task<bool> CrearAsync(Transferencia transferencia)
        {
            if (!transferencia.FechaProgramada.HasValue)
                return false;

            var programacion = new ProgramacionTransferencia
            {
                Transferencia = transferencia,
                FechaProgramada = transferencia.FechaProgramada.Value,
                Ejecutada = false,
                Cancelada = false
            };

            _context.ProgramacionesTransferencias.Add(programacion);
            return await _context.SaveChangesAsync() > 0;
        }

        // Obtener programaciones pendientes para ejecutar
        public async Task<IEnumerable<ProgramacionTransferencia>> ObtenerPendientesAsync()
        {
            return await _context.ProgramacionesTransferencias
                .Include(p => p.Transferencia)
                .Where(p =>
                    !p.Ejecutada &&
                    !p.Cancelada &&
                    p.FechaProgramada <= DateTime.Now)
                .ToListAsync();
        }

        // Obtener una programación por Id (para cancelar, etc.)
        public async Task<ProgramacionTransferencia?> ObtenerPorIdAsync(int id)
        {
            return await _context.ProgramacionesTransferencias
                .Include(p => p.Transferencia)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Actualizar una programación completa
        public async Task<bool> ActualizarAsync(ProgramacionTransferencia programacion)
        {
            _context.ProgramacionesTransferencias.Update(programacion);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> MarcarEjecutada(int id)
        {
            var p = await _context.ProgramacionesTransferencias.FindAsync(id);
            if (p == null)
                return false;

            p.Ejecutada = true;
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> Cancelar(int id)
        {
            var p = await _context.ProgramacionesTransferencias.FindAsync(id);
            if (p == null)
                return false;

            p.Cancelada = true;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
