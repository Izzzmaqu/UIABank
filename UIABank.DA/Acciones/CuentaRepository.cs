using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.DA.Acciones
{
    public class CuentaRepository
    {
        public class MockCuentaRepository : ICuentaRepository
        {
            public Task<Cuenta?> GetByIdAsync(int id)
            {
                // Simula una cuenta con saldo suficiente
                var cuenta = new Cuenta
                {
                    Id = id,
                    Saldo = 1000000m,
                    Estado = "Activa",
                    Moneda = "CRC"
                };
                return Task.FromResult<Cuenta?>(cuenta);
            }
        }
    }
}

