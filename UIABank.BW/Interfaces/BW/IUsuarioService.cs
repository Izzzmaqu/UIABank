using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public class RegistroUsuarioDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public int? ClienteId { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResultDto
    {
        public bool Exitoso { get; set; }
        public string Token { get; set; }
        public string Mensaje { get; set; }
    }

    public interface IUsuarioService
    {
        Task<Usuario> RegistrarUsuarioAsync(RegistroUsuarioDto dto);
        Task<LoginResultDto> AutenticarAsync(LoginDto dto);
    }
}

