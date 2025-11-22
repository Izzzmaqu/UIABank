using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public interface IJwtService
    {
        string GenerarToken(Usuario usuario);
    }
}

