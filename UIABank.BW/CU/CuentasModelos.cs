using UIABank.BC.Modelos;

namespace UIABank.BW.CU
{
    // Lo que llega desde la API para abrir una cuenta
    public class AbrirCuentaRequest
    {
        public Guid ClienteId { get; set; }
        public TipoCuenta Tipo { get; set; }
        public Moneda Moneda { get; set; }
        public decimal SaldoInicial { get; set; }
    }

    // Filtros para búsqueda de cuentas (admin/gestor)
    public class CuentasFiltroRequest
    {
        public Guid? ClienteId { get; set; }
        public TipoCuenta? Tipo { get; set; }
        public Moneda? Moneda { get; set; }
        public EstadoCuenta? Estado { get; set; }
    }

    // Lo que devuelves a la API
    public class CuentaDto
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string Numero { get; set; }
        public TipoCuenta Tipo { get; set; }
        public Moneda Moneda { get; set; }
        public decimal Saldo { get; set; }
        public EstadoCuenta Estado { get; set; }
    }
}
