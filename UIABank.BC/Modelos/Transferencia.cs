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

    
        // Referencia al Módulo B (Cuentas)
        public int CuentaOrigenId { get; set; }

    
        // Referencia al Módulo B (Cuentas)
        public int? CuentaDestinoId { get; set; }

       
        // Referencia al Módulo C (Beneficiarios)
        public int? TerceroId { get; set; }

         
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

        // Clave única de idempotencia: evita que se duplique la misma transferencia(Requerimiento)
        public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();

        // Código único generado para identificar el comprobante de la transferencia.
        public string Referencia { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpper();

        // Id del usuario que ejecuta la transferencia
        // Referencia al Módulo A (Usuarios)
        public int UsuarioEjecutorId { get; set; }

   

  
        public virtual Cuenta? CuentaOrigen { get; set; } // Relación con el módulo B 

  
        public virtual object? CuentaDestino { get; set; }  // Relación con el módulo B 

     
        public virtual object? Tercero { get; set; }        // Relación con el módulo C 
    }
}

//Cambiar el nombre segun de las variables cuando esten los modulos listos
