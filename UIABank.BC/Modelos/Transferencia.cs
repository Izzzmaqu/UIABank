using System;
using System.ComponentModel.DataAnnotations.Schema;

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

        a 
        public decimal Monto { get; set; }

        public decimal Comision { get; set; }

        public string Moneda { get; set; } = "CRC";

        public EstadoTransferencia Estado { get; set; } = EstadoTransferencia.PendienteAprobacion;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaProgramada { get; set; }

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
