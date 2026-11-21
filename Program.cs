using System.Text;
using Avalonia;
using InventarioVideojuegos.Pruebas;

namespace InventarioVideojuegos;

class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        // dotnet run -- --pruebas  -> ejecuta solo los casos de prueba (sin abrir la ventana)
        if (args.Contains("--pruebas"))
        {
            Console.OutputEncoding = Encoding.UTF8;
            CasosDePrueba.EjecutarTodos();
            return 0;
        }

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
