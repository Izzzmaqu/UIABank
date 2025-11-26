using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BW.Cuentas.DTOs;

namespace UIABank.BW.Interfaces.BW
{
    public interface ICuentaService
    {
        Task<Cuenta> AbrirCuentaAsync(AbrirCuentaRequest request);
    }
}
