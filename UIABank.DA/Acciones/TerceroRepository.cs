using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.DA.Acciones
{
    public class TerceroRepository
    {

        public class MockTerceroRepository : ITerceroRepository
        {
            public Task<TerceroTemp?> GetByIdAsync(int id)
            {
                // Simula que el tercero siempre está confirmado
                return Task.FromResult<TerceroTemp?>(new TerceroTemp
                {
                    Id = id,
                    Estado = "Confirmado"
                });
            }
        }
}
