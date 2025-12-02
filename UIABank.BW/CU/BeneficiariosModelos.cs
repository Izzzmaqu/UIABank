using System;
using UIABank.BC.Modelos;

namespace UIABank.BW.CU
{
    public class RegistrarBeneficiarioRequest
    {
        public Guid ClienteId { get; set; }
        public string Alias { get; set; }
        public string Banco { get; set; }
        public Moneda Moneda { get; set; }
        public string NumeroCuenta { get; set; }
        public string Pais { get; set; }
    }

    public class ActualizarAliasBeneficiarioRequest
    {
        public Guid ClienteId { get; set; }
        public string NuevoAlias { get; set; }
    }

    public class BeneficiariosFiltroRequest
    {
        public Guid ClienteId { get; set; }
        public string? Alias { get; set; }
        public string? Banco { get; set; }
        public string? Pais { get; set; }
    }

    public class BeneficiarioDto
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string Alias { get; set; }
        public string Banco { get; set; }
        public Moneda Moneda { get; set; }
        public string NumeroCuenta { get; set; }
        public string Pais { get; set; }
        public EstadoBeneficiario Estado { get; set; }
    }
}
