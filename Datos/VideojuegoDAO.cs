using InventarioVideojuegos.Excepciones;
using InventarioVideojuegos.Modelos;
using Microsoft.Data.Sqlite;

namespace InventarioVideojuegos.Datos;

/// <summary>
/// Acceso a datos con ADO.NET. Es la única clase que escribe SQL.
/// Todas las consultas usan parámetros y cada conexión se cierra con using.
/// </summary>
public class VideojuegoDAO
{
    public void Insertar(Videojuego v)
    {
        try
        {
            using var conexion = BaseDatos.CrearConexion();
            conexion.Open();

            using var comando = conexion.CreateCommand();
            comando.CommandText = @"
                INSERT INTO videojuegos (formato, titulo, plataforma, genero, anio, precio_base, stock)
                VALUES ($formato, $titulo, $plataforma, $genero, $anio, $precio, $stock)";
            AgregarParametros(comando, v);
            comando.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new BaseDatosException("No se pudo guardar el videojuego: " + ex.Message, ex);
        }
    }

    public List<Videojuego> Listar(string filtro)
    {
        var lista = new List<Videojuego>();

        try
        {
            using var conexion = BaseDatos.CrearConexion();
            conexion.Open();

            using var comando = conexion.CreateCommand();
            comando.CommandText = @"
                SELECT id, formato, titulo, plataforma, genero, anio, precio_base, stock
                FROM videojuegos
                WHERE titulo LIKE $filtro OR plataforma LIKE $filtro
                   OR genero LIKE $filtro OR formato LIKE $filtro
                ORDER BY titulo";
            comando.Parameters.AddWithValue("$filtro", "%" + filtro + "%");

            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                Videojuego juego = FabricaVideojuegos.Crear(
                    lector.GetInt32(0),
                    lector.GetString(1),
                    lector.GetString(2),
                    lector.GetString(3),
                    lector.GetString(4),
                    lector.GetInt32(5),
                    (decimal)lector.GetDouble(6),
                    lector.GetInt32(7));
                lista.Add(juego);
            }
        }
        catch (SqliteException ex)
        {
            throw new BaseDatosException("No se pudieron consultar los videojuegos: " + ex.Message, ex);
        }

        return lista;
    }

    /// <summary>Devuelve cuántas filas se modificaron (0 si el id no existe).</summary>
    public int Actualizar(Videojuego v)
    {
        try
        {
            using var conexion = BaseDatos.CrearConexion();
            conexion.Open();

            using var comando = conexion.CreateCommand();
            comando.CommandText = @"
                UPDATE videojuegos
                SET formato = $formato, titulo = $titulo, plataforma = $plataforma, genero = $genero,
                    anio = $anio, precio_base = $precio, stock = $stock
                WHERE id = $id";
            AgregarParametros(comando, v);
            comando.Parameters.AddWithValue("$id", v.Id);
            return comando.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new BaseDatosException("No se pudo actualizar el videojuego: " + ex.Message, ex);
        }
    }

    /// <summary>Devuelve cuántas filas se eliminaron (0 si el id no existe).</summary>
    public int Eliminar(int id)
    {
        try
        {
            using var conexion = BaseDatos.CrearConexion();
            conexion.Open();

            using var comando = conexion.CreateCommand();
            comando.CommandText = "DELETE FROM videojuegos WHERE id = $id";
            comando.Parameters.AddWithValue("$id", id);
            return comando.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new BaseDatosException("No se pudo eliminar el videojuego: " + ex.Message, ex);
        }
    }

    /// <summary>Indica si ya hay otro registro con el mismo título, plataforma y formato.</summary>
    public bool ExisteDuplicado(string titulo, string plataforma, string formato, int idExcluido)
    {
        try
        {
            using var conexion = BaseDatos.CrearConexion();
            conexion.Open();

            using var comando = conexion.CreateCommand();
            comando.CommandText = @"
                SELECT COUNT(*) FROM videojuegos
                WHERE lower(titulo) = lower($titulo) AND plataforma = $plataforma
                  AND formato = $formato AND id <> $id";
            comando.Parameters.AddWithValue("$titulo", titulo);
            comando.Parameters.AddWithValue("$plataforma", plataforma);
            comando.Parameters.AddWithValue("$formato", formato);
            comando.Parameters.AddWithValue("$id", idExcluido);

            long cantidad = Convert.ToInt64(comando.ExecuteScalar());
            return cantidad > 0;
        }
        catch (SqliteException ex)
        {
            throw new BaseDatosException("No se pudo verificar el videojuego: " + ex.Message, ex);
        }
    }

    private static void AgregarParametros(SqliteCommand comando, Videojuego v)
    {
        comando.Parameters.AddWithValue("$formato", v.Formato);
        comando.Parameters.AddWithValue("$titulo", v.Titulo);
        comando.Parameters.AddWithValue("$plataforma", v.Plataforma);
        comando.Parameters.AddWithValue("$genero", v.Genero);
        comando.Parameters.AddWithValue("$anio", v.Anio);
        comando.Parameters.AddWithValue("$precio", (double)v.PrecioBase);
        comando.Parameters.AddWithValue("$stock", v.Stock);
    }
}
