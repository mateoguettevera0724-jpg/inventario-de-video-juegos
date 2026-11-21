using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using InventarioVideojuegos.Datos;
using InventarioVideojuegos.Excepciones;
using InventarioVideojuegos.Modelos;
using InventarioVideojuegos.Servicios;
using InventarioVideojuegos.Utilidades;

namespace InventarioVideojuegos.Vistas;

/// <summary>
/// Ventana principal. Solo lee el formulario, responde a los eventos y muestra los datos;
/// las reglas del programa están en VideojuegoServicio.
/// </summary>
public partial class MainWindow : Window
{
    private readonly VideojuegoServicio _servicio;
    private int _idSeleccionado = 0;

    public MainWindow()
    {
        InitializeComponent();

        CmbPlataforma.ItemsSource = new[]
        {
            "PC", "PlayStation 5", "PlayStation 4", "Xbox Series", "Nintendo Switch"
        };
        CmbGenero.ItemsSource = new[]
        {
            "Acción", "Aventura", "RPG", "Deportes", "Shooter", "Estrategia", "Terror", "Carreras", "Otro"
        };
        CmbFormato.ItemsSource = FabricaVideojuegos.Formatos;

        _servicio = new VideojuegoServicio();

        try
        {
            BaseDatos.Inicializar();
            CargarLista();
        }
        catch (Exception ex)
        {
            MostrarMensaje("No se pudo abrir la base de datos: " + ex.Message, true);
        }
    }

    private void CargarLista(string filtro = "")
    {
        GridJuegos.ItemsSource = _servicio.Listar(filtro);
        ActualizarResumen();
    }

    private void ActualizarResumen()
    {
        ResumenInventario resumen = _servicio.ObtenerResumen();
        TxtResumen.Text = $"Títulos: {resumen.TotalTitulos}   |   Unidades en stock: {resumen.UnidadesEnStock}   |   " +
                          $"Valor del inventario: {FormatoMoneda.Texto(resumen.ValorTotal)}";
    }

    private void MostrarMensaje(string texto, bool esError)
    {
        TxtMensaje.Text = texto;
        TxtMensaje.Foreground = esError ? Brushes.Firebrick : Brushes.SeaGreen;
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        TxtTitulo.Text = "";
        CmbPlataforma.SelectedItem = null;
        CmbGenero.SelectedItem = null;
        CmbFormato.SelectedItem = null;
        TxtAnio.Text = "";
        TxtPrecio.Text = "";
        TxtStock.Text = "";
        GridJuegos.SelectedItem = null;
    }

    /// <summary>Convierte lo escrito en el formulario en un objeto Videojuego. Lanza ValidacionException si hay errores.</summary>
    private Videojuego LeerFormulario(int id)
    {
        if (!int.TryParse(TxtAnio.Text, out int anio))
            throw new ValidacionException("El año debe ser un número entero.");

        if (!decimal.TryParse(TxtPrecio.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal precio))
            throw new ValidacionException("El precio debe ser un número válido.");

        if (!int.TryParse(TxtStock.Text, out int stock))
            throw new ValidacionException("El stock debe ser un número entero.");

        string titulo = TxtTitulo.Text ?? "";
        string plataforma = CmbPlataforma.SelectedItem as string ?? "";
        string genero = CmbGenero.SelectedItem as string ?? "";
        string formato = CmbFormato.SelectedItem as string ?? "";

        return FabricaVideojuegos.Crear(id, formato, titulo, plataforma, genero, anio, precio, stock);
    }

    private void BtnGuardar_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Videojuego nuevo = LeerFormulario(0);
            _servicio.Registrar(nuevo);
            CargarLista();
            LimpiarFormulario();
            MostrarMensaje("Videojuego registrado correctamente.", false);
        }
        catch (InventarioException ex)
        {
            MostrarMensaje(ex.Message, true);
        }
        catch (Exception ex)
        {
            MostrarMensaje("Ocurrió un error inesperado: " + ex.Message, true);
        }
    }

    private void BtnActualizar_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_idSeleccionado == 0)
                throw new ValidacionException("Seleccione un videojuego de la tabla para modificarlo.");

            Videojuego modificado = LeerFormulario(_idSeleccionado);
            _servicio.Modificar(modificado);
            CargarLista();
            LimpiarFormulario();
            MostrarMensaje("Videojuego actualizado correctamente.", false);
        }
        catch (InventarioException ex)
        {
            MostrarMensaje(ex.Message, true);
        }
        catch (Exception ex)
        {
            MostrarMensaje("Ocurrió un error inesperado: " + ex.Message, true);
        }
    }

    private async void BtnEliminar_Click(object? sender, RoutedEventArgs e)
    {
        Videojuego? seleccionado = GridJuegos.SelectedItem as Videojuego;
        if (seleccionado == null)
        {
            MostrarMensaje("Seleccione un videojuego de la tabla para eliminarlo.", true);
            return;
        }

        var ventana = new ConfirmacionWindow($"¿Desea eliminar \"{seleccionado.Titulo}\" ({seleccionado.Plataforma}, {seleccionado.Formato})?");
        bool confirmado = await ventana.ShowDialog<bool>(this);
        if (!confirmado)
        {
            return;
        }

        try
        {
            _servicio.Eliminar(seleccionado.Id);
            CargarLista(TxtBuscar.Text ?? "");
            LimpiarFormulario();
            MostrarMensaje("Videojuego eliminado correctamente.", false);
        }
        catch (InventarioException ex)
        {
            MostrarMensaje(ex.Message, true);
        }
        catch (Exception ex)
        {
            MostrarMensaje("Ocurrió un error inesperado: " + ex.Message, true);
        }
    }

    private void BtnLimpiar_Click(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        MostrarMensaje("", false);
    }

    private void BtnBuscar_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            string filtro = TxtBuscar.Text ?? "";
            IReadOnlyList<Videojuego> resultado = _servicio.Listar(filtro);
            GridJuegos.ItemsSource = resultado;
            MostrarMensaje($"Se encontraron {resultado.Count} resultado(s).", false);
        }
        catch (InventarioException ex)
        {
            MostrarMensaje(ex.Message, true);
        }
    }

    private void BtnVerTodos_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            TxtBuscar.Text = "";
            CargarLista();
            MostrarMensaje("", false);
        }
        catch (InventarioException ex)
        {
            MostrarMensaje(ex.Message, true);
        }
    }

    private void GridJuegos_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Videojuego? v = GridJuegos.SelectedItem as Videojuego;
        if (v == null)
        {
            return;
        }

        _idSeleccionado = v.Id;
        TxtTitulo.Text = v.Titulo;
        CmbPlataforma.SelectedItem = v.Plataforma;
        CmbGenero.SelectedItem = v.Genero;
        CmbFormato.SelectedItem = v.Formato;
        TxtAnio.Text = v.Anio.ToString();
        TxtPrecio.Text = v.PrecioBase.ToString("0.##", CultureInfo.CurrentCulture);
        TxtStock.Text = v.Stock.ToString();
    }
}
