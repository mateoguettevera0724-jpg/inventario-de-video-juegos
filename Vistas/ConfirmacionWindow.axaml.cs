using Avalonia.Controls;
using Avalonia.Interactivity;

namespace InventarioVideojuegos.Vistas;

/// <summary>Ventana pequeña que pregunta si se desea eliminar. Devuelve true si el usuario confirma.</summary>
public partial class ConfirmacionWindow : Window
{
    public ConfirmacionWindow()
    {
        InitializeComponent();
    }

    public ConfirmacionWindow(string pregunta) : this()
    {
        TxtPregunta.Text = pregunta;
    }

    private void BtnSi_Click(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void BtnNo_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
