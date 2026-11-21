using System.Globalization;

namespace InventarioVideojuegos.Utilidades;

/// <summary>
/// Utilidad de presentación. Los precios se manejan siempre como decimal
/// y solo se convierten a texto cuando se muestran.
/// </summary>
public static class FormatoMoneda
{
    private static readonly CultureInfo Cultura = ObtenerCultura();

    private static CultureInfo ObtenerCultura()
    {
        try
        {
            return new CultureInfo("es-CO");
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }

    /// <summary>Devuelve el valor con formato de moneda, por ejemplo $57.500.</summary>
    public static string Texto(decimal valor) => "$" + valor.ToString("N0", Cultura);
}
