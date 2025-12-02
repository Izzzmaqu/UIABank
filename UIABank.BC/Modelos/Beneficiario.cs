using System;

namespace UIABank.BC.Modelos
{
    public class Beneficiario
    {
        public Guid Id { get; private set; }

        public Guid ClienteId { get; private set; }

        public string Alias { get; private set; }

        public string Banco { get; private set; }

        public Moneda Moneda { get; private set; }

        public string NumeroCuenta { get; private set; }

        public string Pais { get; private set; }

        public EstadoBeneficiario Estado { get; private set; }

        // EF Core
        private Beneficiario() { }

        public Beneficiario(Guid clienteId, string alias, string banco, Moneda moneda, string numeroCuenta, string pais)
        {
            if (clienteId == Guid.Empty)
                throw new ArgumentException("El cliente es requerido.", nameof(clienteId));

            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("El alias es requerido.", nameof(alias));

            if (alias.Length < 3 || alias.Length > 30)
                throw new ArgumentException("El alias debe tener entre 3 y 30 caracteres.", nameof(alias));

            if (string.IsNullOrWhiteSpace(banco))
                throw new ArgumentException("El banco es requerido.", nameof(banco));

            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("El número de cuenta es requerido.", nameof(numeroCuenta));

            if (numeroCuenta.Length < 12 || numeroCuenta.Length > 20 || !EsSoloDigitos(numeroCuenta))
                throw new ArgumentException("El número de cuenta debe tener entre 12 y 20 dígitos.", nameof(numeroCuenta));

            if (string.IsNullOrWhiteSpace(pais))
                throw new ArgumentException("El país es requerido.", nameof(pais));

            Id = Guid.NewGuid();
            ClienteId = clienteId;
            Alias = alias.Trim();
            Banco = banco.Trim();
            Moneda = moneda;
            NumeroCuenta = numeroCuenta;
            Pais = pais.Trim();
            Estado = EstadoBeneficiario.Inactivo;
        }

        public void Confirmar()
        {
            if (Estado == EstadoBeneficiario.Activo)
                return;

            Estado = EstadoBeneficiario.Activo;
        }

        public void CambiarAlias(string nuevoAlias)
        {
            if (string.IsNullOrWhiteSpace(nuevoAlias))
                throw new ArgumentException("El alias es requerido.", nameof(nuevoAlias));

            if (nuevoAlias.Length < 3 || nuevoAlias.Length > 30)
                throw new ArgumentException("El alias debe tener entre 3 y 30 caracteres.", nameof(nuevoAlias));

            Alias = nuevoAlias.Trim();
        }

        private static bool EsSoloDigitos(string valor)
        {
            foreach (var c in valor)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
}
