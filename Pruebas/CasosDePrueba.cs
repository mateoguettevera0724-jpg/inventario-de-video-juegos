using InventarioVideojuegos.Datos;
using InventarioVideojuegos.Excepciones;
using InventarioVideojuegos.Modelos;
using InventarioVideojuegos.Servicios;
using Microsoft.Data.Sqlite;

namespace InventarioVideojuegos.Pruebas;

/// <summary>
/// Casos de prueba que se ejecutan con: dotnet run -- --pruebas
/// Usan un archivo de base de datos temporal, así que no tocan los datos reales del inventario.
/// </summary>
public static class CasosDePrueba
{
    private static int _correctas;
    private static int _fallidas;

    public static void EjecutarTodos()
    {
        _correctas = 0;
        _fallidas = 0;

        string archivo = Path.Combine(Path.GetTempPath(), "inventario_pruebas.db");
        BorrarArchivo(archivo);

        BaseDatos.UsarArchivo(archivo);
        BaseDatos.Inicializar();
        var servicio = new VideojuegoServicio();

        Console.WriteLine("=== CASOS DE PRUEBA - INVENTARIO DE VIDEOJUEGOS ===");

        PruebaRegistro(servicio);
        PruebaConsulta(servicio);
        PruebaActualizacion(servicio);
        PruebaEliminacion(servicio);
        PruebaValidacion(servicio);
        PruebaPersistencia();
        PruebaResumen();

        Console.WriteLine();
        Console.WriteLine($"Resultado: {_correctas} correctas, {_fallidas} fallidas.");

        BorrarArchivo(archivo);
    }

    private static void PruebaRegistro(VideojuegoServicio servicio)
    {
        Console.WriteLine();
        Console.WriteLine("Prueba 1 - Registro");

        servicio.Registrar(new VideojuegoFisico(0, "Zelda Tears of the Kingdom", "Nintendo Switch", "Aventura", 2023, 250000m, 5));

        var lista = servicio.Listar();
        Verificar("El videojuego quedó guardado en la base de datos", lista.Any(v => v.Titulo == "Zelda Tears of the Kingdom"));
        Verificar("Se le asignó un identificador", lista.Count > 0 && lista[0].Id > 0);
    }

    private static void PruebaConsulta(VideojuegoServicio servicio)
    {
        Console.WriteLine();
        Console.WriteLine("Prueba 2 - Consulta");

        var digital = new VideojuegoDigital(0, "Hades", "PC", "Acción", 2020, 60000m, 10);
        servicio.Registrar(digital);

        Verificar("Se muestran los 2 registros almacenados", servicio.Listar().Count == 2);
        Verificar("La búsqueda por título encuentra 1 resultado", servicio.Listar("hades").Count == 1);
        Verificar("La búsqueda por plataforma encuentra 1 resultado", servicio.Listar("Switch").Count == 1);
        Verificar("El precio de venta físico es el precio base", new VideojuegoFisico(0, "A", "PC", "RPG", 2020, 100000m, 1).PrecioVenta == 100000m);
        Verificar("El precio de venta digital tiene 10 % de descuento", digital.PrecioVenta == 54000m);
    }

    private static void PruebaActualizacion(VideojuegoServicio servicio)
    {
        Console.WriteLine();
        Console.WriteLine("Prueba 3 - Actualización");

        Videojuego guardado = servicio.Listar("Zelda")[0];
        var modificado = new VideojuegoFisico(guardado.Id, guardado.Titulo, guardado.Plataforma,
                                              guardado.Genero, guardado.Anio, 200000m, 8);
        servicio.Modificar(modificado);

        Videojuego releido = servicio.Listar("Zelda")[0];
        Verificar("El precio nuevo se guardó (200000)", releido.PrecioBase == 200000m);
        Verificar("El stock nuevo se guardó (8)", releido.Stock == 8);
    }

    private static void PruebaEliminacion(VideojuegoServicio servicio)
    {
        Console.WriteLine();
        Console.WriteLine("Prueba 4 - Eliminación");

        Videojuego hades = servicio.Listar("Hades")[0];
        servicio.Eliminar(hades.Id);

        Verificar("El registro dejó de aparecer", servicio.Listar("Hades").Count == 0);
        Verificar("Solo queda 1 registro", servicio.Listar().Count == 1);
    }

    private static void PruebaValidacion(VideojuegoServicio servicio)
    {
        Console.WriteLine();
        Console.WriteLine("Prueba 5 - Validación (el programa informa el error y no se cierra)");

        EsperarExcepcion<ValidacionException>("Título vacío",
            () => new VideojuegoFisico(0, "  ", "PC", "RPG", 2020, 50000m, 1));
        EsperarExcepcion<ValidacionException>("Año fuera de rango (1800)",
            () => new VideojuegoFisico(0, "Juego", "PC", "RPG", 1800, 50000m, 1));
        EsperarExcepcion<ValidacionException>("Precio en cero",
            () => new VideojuegoFisico(0, "Juego", "PC", "RPG", 2020, 0m, 1));
        EsperarExcepcion<ValidacionException>("Stock negativo",
            () => new VideojuegoDigital(0, "Juego", "PC", "RPG", 2020, 50000m, -3));
        EsperarExcepcion<ValidacionException>("Formato no seleccionado",
            () => FabricaVideojuegos.Crear(0, "", "Juego", "PC", "RPG", 2020, 50000m, 1));
        EsperarExcepcion<VideojuegoDuplicadoException>("Registrar un videojuego repetido",
            () => servicio.Registrar(new VideojuegoFisico(0, "zelda tears of the kingdom", "Nintendo Switch", "Aventura", 2023, 250000m, 2)));
        EsperarExcepcion<VideojuegoNoEncontradoException>("Eliminar un identificador inexistente",
            () => servicio.Eliminar(9999));
    }

    private static void PruebaPersistencia()
    {
        Console.WriteLine();
        Console.WriteLine("Prueba 6 - Persistencia (simula cerrar y abrir el programa)");

        SqliteConnection.ClearAllPools();
        var servicioNuevo = new VideojuegoServicio();

        Verificar("Con un servicio nuevo, el registro sigue guardado", servicioNuevo.Listar("Zelda").Count == 1);
    }

    private static void PruebaResumen()
    {
        Console.WriteLine();
        Console.WriteLine("Prueba 7 - Resumen del inventario");

        var servicio = new VideojuegoServicio();
        ResumenInventario resumen = servicio.ObtenerResumen();

        Verificar("Total de títulos = 1", resumen.TotalTitulos == 1);
        Verificar("Unidades en stock = 8", resumen.UnidadesEnStock == 8);
        Verificar("Valor total = 1600000 (200000 x 8)", resumen.ValorTotal == 1600000m);
    }

    private static void Verificar(string descripcion, bool condicion)
    {
        if (condicion)
        {
            _correctas++;
            Console.WriteLine($"  [OK]     {descripcion}");
        }
        else
        {
            _fallidas++;
            Console.WriteLine($"  [FALLÓ]  {descripcion}");
        }
    }

    private static void EsperarExcepcion<T>(string descripcion, Action accion) where T : Exception
    {
        try
        {
            accion();
            _fallidas++;
            Console.WriteLine($"  [FALLÓ]  {descripcion}: no lanzó ninguna excepción");
        }
        catch (T ex)
        {
            _correctas++;
            Console.WriteLine($"  [OK]     {descripcion}: {ex.Message}");
        }
        catch (Exception ex)
        {
            _fallidas++;
            Console.WriteLine($"  [FALLÓ]  {descripcion}: se lanzó {ex.GetType().Name}");
        }
    }

    private static void BorrarArchivo(string ruta)
    {
        try
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(ruta))
                File.Delete(ruta);
        }
        catch (IOException)
        {
        }
    }
}
