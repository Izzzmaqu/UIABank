
namespace UIABank.BC.Cuentas
{
    public enum TipoCuenta
    {
        Ahorros = 1,
        Corriente = 2,
        Inversion = 3,
        PlazoFijo = 4
    }

    public enum Moneda
    {
        CRC = 1,
        USD = 2
    }

    public enum EstadoCuenta
    {
        Activa = 1,
        Bloqueada = 2,
        Cerrada = 3
    }
}