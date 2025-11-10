using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UIABank.DA.Entidades
{
    [Table("Transferencia")]
    public class TransferenciaEntidad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CuentaOrigenId { get; set; }

        public int? CuentaDestinoId { get; set; }

        public int? TerceroId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Comision { get; set; }

        [Required]
        [StringLength(3)]
        public string Moneda { get; set; } = "CRC";

        [Required]
        public string Estado { get; set; } = "PendienteAprobacion";

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaEjecucion { get; set; }

        [Required]
        public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Referencia { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpper();

        [Required]
        public int UsuarioEjecutorId { get; set; }

       
  
        public virtual CuentaEntidad? CuentaOrigen { get; set; }
        public virtual CuentaEntidad? CuentaDestino { get; set; }
    }
}
