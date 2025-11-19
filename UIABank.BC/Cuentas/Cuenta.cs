using System;

namespace UIABank.BC.Cuentas
{
    public class Cuenta
    {
        // Clave primaria
        public Guid Id { get; private set; }

        // Número de 12 dígitos
        public string Numero { get; private set; }

        public TipoCuenta Tipo { get; private set; }
        public Moneda Moneda { get; private set; }

        public decimal Saldo { get; private set; }

        public EstadoCuenta Estado { get; private set; }

        // Relación con Cliente (del módulo A)
        public Guid ClienteId { get; private set; }
        // public Cliente Cliente { get; private set; } // si ya tienes entity Cliente en BC

        // EF Core necesita un constructor sin parámetros
        private Cuenta() { }

        public Cuenta(Guid clienteId, string numero, TipoCuenta tipo, Moneda moneda, decimal saldoInicial)
        {
            if (clienteId == Guid.Empty)
                throw new ArgumentException("El cliente es requerido.", nameof(clienteId));

            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("El número de cuenta es requerido.", nameof(numero));

            if (numero.Length != 12 || !EsSoloDigitos(numero))
                throw new ArgumentException("El número de cuenta debe tener exactamente 12 dígitos.", nameof(numero));

            if (saldoInicial < 0)
                throw new ArgumentException("El saldo inicial no puede ser negativo.", nameof(saldoInicial));

            Id = Guid.NewGuid();
            ClienteId = clienteId;
            Numero = numero;
            Tipo = tipo;
            Moneda = moneda;
            Saldo = saldoInicial;
            Estado = EstadoCuenta.Activa;
        }

        public void Debitar(decimal monto)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto a debitar debe ser mayor a cero.", nameof(monto));

            if (Estado != EstadoCuenta.Activa)
                throw new InvalidOperationException("Solo se puede debitar desde cuentas activas.");

            if (Saldo < monto)
                throw new InvalidOperationException("Saldo insuficiente.");

            Saldo -= monto;
        }

        public void Acreditar(decimal monto)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto a acreditar debe ser mayor a cero.", nameof(monto));

            if (Estado != EstadoCuenta.Activa)
                throw new InvalidOperationException("Solo se puede acreditar en cuentas activas.");

            Saldo += monto;
        }

        public void Bloquear()
        {
            if (Estado == EstadoCuenta.Cerrada)
                throw new InvalidOperationException("No se puede bloquear una cuenta cerrada.");

            Estado = EstadoCuenta.Bloqueada;
        }

        public void Cerrar()
        {
            if (Saldo != 0)
                throw new InvalidOperationException("La cuenta solo se puede cerrar si el saldo es 0.");

            Estado = EstadoCuenta.Cerrada;
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