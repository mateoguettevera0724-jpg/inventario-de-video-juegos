using InventarioVideojuegos.Excepciones;
using Microsoft.Data.Sqlite;

namespace InventarioVideojuegos.Datos;

/// <summary>Crea las conexiones con SQLite y prepara la tabla la primera vez que se ejecuta el programa.</summary>
public static class BaseDatos
{
    private static string _ruta = Path.Combine(AppContext.BaseDirectory, "inventario.db");

    public static string Ruta => _ruta;

    /// <summary>Cambia el archivo de la base de datos (lo usan los casos de prueba).</summary>
    public static void UsarArchivo(string ruta)
    {
        _ruta = ruta;
    }

    public static SqliteConnection CrearConexion()
    {
        return new SqliteConnection($"Data Source={_ruta}");
    }

    public static void Inicializar()
    {
        try
        {
            using var conexion = CrearConexion();
            conexion.Open();

            using var comando = conexion.CreateCommand();
            comando.CommandText = @"
                CREATE TABLE IF NOT EXISTS videojuegos (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    formato TEXT NOT NULL,
                    titulo TEXT NOT NULL,
                    plataforma TEXT NOT NULL,
                    genero TEXT NOT NULL,
                    anio INTEGER NOT NULL,
                    precio_base REAL NOT NULL,
                    stock INTEGER NOT NULL
                );";
            comando.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new BaseDatosException("No se pudo crear o abrir la base de datos: " + ex.Message, ex);
        }
    }
}
