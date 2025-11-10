using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BC.Modelos
{
  
    public class Transferencia
    {
   
        public int Id { get; set; }

        // FK que indica desde qué cuenta se envía el dinero.
        public int CuentaOrigenId { get; set; }

        // FK opcional: si la transferencia es entre cuentas del mismo cliente.
        public int? CuentaDestinoId { get; set; }

        // FK opcional: si la transferencia es hacia un tercero (otra persona).
        public int? TerceroId { get; set; }

        // Monto principal de la transferencia (sin comisiones).
        public decimal Monto { get; set; }

        // Monto que representa la comisión aplicada a la transferencia.
        public decimal Comision { get; set; }

        // Moneda de la transferencia 
        public string Moneda { get; set; } = "CRC";

        // Estado actual de la transferencia, definido por el enum en la clase EstadoTransefrencia.
        public EstadoTransferencia Estado { get; set; } = EstadoTransferencia.PendienteAprobacion;

        // Fecha en que se crea la solicitud de transferencia.
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Fecha programada para ejecución (solo aplica si es una transferencia programada).
        public DateTime? FechaProgramada { get; set; }

        // Fecha real en la que se ejecuta la transferencia (cuando ocurre la transacción).
        public DateTime? FechaEjecucion { get; set; }

        // Clave única de idempotencia: evita que se duplique la misma transferencia(requisito del Proyecto)
      
        public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();

        // Código único generado para identificar el comprobante de la transferencia.
        public string Referencia { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpper();

        // Id del usuario que ejecuta la transferencia (se obtiene del token JWT).
        public int UsuarioEjecutorId { get; set; }

        // Relaciones de navegación con otras entidades (solo lectura desde EF Core).
        public virtual Cuenta? CuentaOrigen { get; set; }  // Relación con la cuenta de origen.
        public virtual Cuenta? CuentaDestino { get; set; } // Relación con la cuenta de destino.
        public virtual Tercero? Tercero { get; set; }      // Relación con el tercero, si aplica.
    }
}
