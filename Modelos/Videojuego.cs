using InventarioVideojuegos.Excepciones;
using InventarioVideojuegos.Utilidades;

namespace InventarioVideojuegos.Modelos;

/// <summary>
/// Clase abstracta con los datos comunes de cualquier videojuego del inventario.
/// Cada formato (físico o digital) define su propia regla en <see cref="CalcularPrecioVenta"/>.
/// </summary>
public abstract class Videojuego
{
    public int Id { get; }
    public string Titulo { get; }
    public string Plataforma { get; }
    public string Genero { get; }
    public int Anio { get; }
    public decimal PrecioBase { get; }
    public int Stock { get; }

    /// <summary>Formato del videojuego (Físico o Digital).</summary>
    public abstract string Formato { get; }

    public decimal PrecioVenta => CalcularPrecioVenta();
    public string PrecioBaseTexto => FormatoMoneda.Texto(PrecioBase);
    public string PrecioVentaTexto => FormatoMoneda.Texto(PrecioVenta);

    protected Videojuego(int id, string titulo, string plataforma, string genero, int anio, decimal precioBase, int stock)
    {
        if (id < 0)
            throw new ValidacionException("El identificador del videojuego no es válido.");
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ValidacionException("El título es obligatorio.");
        if (titulo.Trim().Length > 100)
            throw new ValidacionException("El título no puede tener más de 100 caracteres.");
        if (string.IsNullOrWhiteSpace(plataforma))
            throw new ValidacionException("Debe seleccionar una plataforma.");
        if (string.IsNullOrWhiteSpace(genero))
            throw new ValidacionException("Debe seleccionar un género.");

        int anioMaximo = DateTime.Now.Year + 1;
        if (anio < 1970 || anio > anioMaximo)
            throw new ValidacionException($"El año debe estar entre 1970 y {anioMaximo}.");
        if (precioBase <= 0)
            throw new ValidacionException("El precio debe ser mayor que cero.");
        if (stock < 0)
            throw new ValidacionException("El stock no puede ser negativo.");

        Id = id;
        Titulo = titulo.Trim();
        Plataforma = plataforma.Trim();
        Genero = genero.Trim();
        Anio = anio;
        PrecioBase = precioBase;
        Stock = stock;
    }

    /// <summary>Precio al que se vende el videojuego. Cada clase derivada implementa su propia regla.</summary>
    public abstract decimal CalcularPrecioVenta();

    public override string ToString() => $"{Titulo} ({Plataforma}, {Formato})";
}
