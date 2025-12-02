namespace UIABank.BC.Modelos
{
    public class Transferencia
    {
        public int Id { get; set; }

        // Origen (siempre)
        public Guid CuentaOrigenId { get; set; }
        public virtual Cuenta? CuentaOrigen { get; set; }

        // Destino (si es a otra cuenta interna)
        public Guid? CuentaDestinoId { get; set; }
        public virtual Cuenta? CuentaDestino { get; set; }

        // Tercero (si es a beneficiario)
        public Guid? TerceroId { get; set; }
        public virtual Beneficiario? Tercero { get; set; }

        public decimal Monto { get; set; }
        public decimal Comision { get; set; }

        public string Moneda { get; set; } = "CRC";
        public EstadoTransferencia Estado { get; set; } = EstadoTransferencia.PendienteAprobacion;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaEjecucion { get; set; }

        public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString();
        public string Referencia { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpper();

        public int UsuarioEjecutorId { get; set; }
        // (Opcional) si tenés entidad Usuario:
        // public virtual Usuario? UsuarioEjecutor { get; set; }
    }
}
