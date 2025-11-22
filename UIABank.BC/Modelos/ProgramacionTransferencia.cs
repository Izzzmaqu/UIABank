using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.BC.Modelos
{
    public class ProgramacionTransferencia
    {

        public int Id { get; set; }
        public int TransferenciaId { get; set; }

        public DateTime FechaProgramada { get; set; }
        public bool Ejecutada { get; set; } = false;
        public bool Cancelada { get; set; } = false;

        public virtual Transferencia? Transferencia { get; set; }
    }
}
