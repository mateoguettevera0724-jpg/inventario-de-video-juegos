using InventarioVideojuegos.Excepciones;

namespace InventarioVideojuegos.Modelos;

/// <summary>
/// Crea el tipo de videojuego correcto según el formato. Se usa al leer de la base de datos
/// y al leer el formulario, para no repetir el mismo switch en varios lugares.
/// </summary>
public static class FabricaVideojuegos
{
    public static readonly string[] Formatos = { "Físico", "Digital" };

    public static Videojuego Crear(int id, string formato, string titulo, string plataforma,
                                   string genero, int anio, decimal precioBase, int stock)
    {
        switch (formato)
        {
            case "Físico":
                return new VideojuegoFisico(id, titulo, plataforma, genero, anio, precioBase, stock);
            case "Digital":
                return new VideojuegoDigital(id, titulo, plataforma, genero, anio, precioBase, stock);
            default:
                throw new ValidacionException("Debe seleccionar un formato (Físico o Digital).");
        }
    }
}
