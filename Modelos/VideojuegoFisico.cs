namespace InventarioVideojuegos.Modelos;

/// <summary>Videojuego en disco o cartucho: se vende al precio base.</summary>
public class VideojuegoFisico : Videojuego
{
    public override string Formato => "Físico";

    public VideojuegoFisico(int id, string titulo, string plataforma, string genero, int anio, decimal precioBase, int stock)
        : base(id, titulo, plataforma, genero, anio, precioBase, stock)
    {
    }

    public override decimal CalcularPrecioVenta() => PrecioBase;
}
