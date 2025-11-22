using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.DA
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObtenerPorEmailAsync(string email);
        Task<bool> ExisteEmailAsync(string email);
        Task<Usuario> CrearAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
        Task<Usuario> ObtenerPorIdAsync(int id);
    }
}

