using InventarioVideojuegos.Datos;
using InventarioVideojuegos.Excepciones;
using InventarioVideojuegos.Modelos;

namespace InventarioVideojuegos.Servicios;

/// <summary>
/// Lógica del programa: aplica las reglas (duplicados, existencia del registro)
/// y llama al DAO. La ventana nunca habla directamente con la base de datos.
/// </summary>
public class VideojuegoServicio
{
    private readonly VideojuegoDAO _dao = new VideojuegoDAO();

    public IReadOnlyList<Videojuego> Listar(string filtro = "")
    {
        return _dao.Listar((filtro ?? "").Trim());
    }

    public void Registrar(Videojuego v)
    {
        ArgumentNullException.ThrowIfNull(v);

        if (_dao.ExisteDuplicado(v.Titulo, v.Plataforma, v.Formato, 0))
            throw new VideojuegoDuplicadoException(v.Titulo, v.Plataforma, v.Formato);

        _dao.Insertar(v);
    }

    public void Modificar(Videojuego v)
    {
        ArgumentNullException.ThrowIfNull(v);

        if (v.Id <= 0)
            throw new ValidacionException("Seleccione un videojuego de la tabla para modificarlo.");

        if (_dao.ExisteDuplicado(v.Titulo, v.Plataforma, v.Formato, v.Id))
            throw new VideojuegoDuplicadoException(v.Titulo, v.Plataforma, v.Formato);

        int filas = _dao.Actualizar(v);
        if (filas == 0)
            throw new VideojuegoNoEncontradoException(v.Id);
    }

    public void Eliminar(int id)
    {
        if (id <= 0)
            throw new ValidacionException("Seleccione un videojuego de la tabla para eliminarlo.");

        int filas = _dao.Eliminar(id);
        if (filas == 0)
            throw new VideojuegoNoEncontradoException(id);
    }

    public ResumenInventario ObtenerResumen()
    {
        List<Videojuego> todos = _dao.Listar("");
        return new ResumenInventario(
            todos.Count,
            todos.Sum(v => v.Stock),
            todos.Sum(v => v.PrecioVenta * v.Stock));
    }
}
