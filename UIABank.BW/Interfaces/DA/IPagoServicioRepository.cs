using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.DA
{
    public interface IPagoServicioRepository
    {
        Task<PagoServicio> CrearAsync(PagoServicio pago);
        Task<PagoServicio> ObtenerPorIdAsync(int id);
        Task<List<PagoServicio>> ObtenerPorClienteAsync(
            int clienteId,
            DateTime? desde,
            DateTime? hasta,
            bool soloProgramados);

        Task ActualizarAsync(PagoServicio pago);
        Task<List<PagoServicio>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta);
    }
}

