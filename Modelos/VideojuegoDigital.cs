namespace InventarioVideojuegos.Modelos;

/// <summary>Videojuego en descarga digital: tiene un descuento del 10 % sobre el precio base.</summary>
public class VideojuegoDigital : Videojuego
{
    private const decimal Descuento = 0.10m;

    public override string Formato => "Digital";

    public VideojuegoDigital(int id, string titulo, string plataforma, string genero, int anio, decimal precioBase, int stock)
        : base(id, titulo, plataforma, genero, anio, precioBase, stock)
    {
    }

    public override decimal CalcularPrecioVenta() => PrecioBase * (1 - Descuento);
}
