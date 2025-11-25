using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public class CrearProveedorServicioDto
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int MinLongitudContrato { get; set; }
        public int MaxLongitudContrato { get; set; }
    }

    public class ActualizarProveedorServicioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int MinLongitudContrato { get; set; }
        public int MaxLongitudContrato { get; set; }
        public bool Activo { get; set; }
    }

    public interface IProveedorServicioService
    {
        Task<ProveedorServicio> CrearProveedorAsync(CrearProveedorServicioDto dto);
        Task<ProveedorServicio> ActualizarProveedorAsync(ActualizarProveedorServicioDto dto);
        Task<List<ProveedorServicio>> ObtenerTodosAsync();
        Task<ProveedorServicio> ObtenerPorIdAsync(int id);
    }
}

