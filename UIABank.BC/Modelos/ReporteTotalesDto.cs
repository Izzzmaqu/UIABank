using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.BC.Modelos.Reportes
{
    public class ReporteTotalesDto
    {
        public int TotalTransferencias { get; set; }
        public decimal MontoTransferencias { get; set; }

        public int TotalPagos { get; set; }
        public decimal MontoPagos { get; set; }

        public decimal TotalComisiones { get; set; }
    }
}
