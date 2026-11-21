namespace InventarioVideojuegos.Excepciones;

/// <summary>Excepción base para los errores propios del inventario.</summary>
public class InventarioException : Exception
{
    public InventarioException(string mensaje) : base(mensaje) { }

    public InventarioException(string mensaje, Exception interna) : base(mensaje, interna) { }
}

public class ValidacionException : InventarioException
{
    public ValidacionException(string mensaje) : base(mensaje) { }
}

public class VideojuegoNoEncontradoException : InventarioException
{
    public VideojuegoNoEncontradoException(int id)
        : base($"No existe un videojuego con el identificador {id}.") { }
}

public class VideojuegoDuplicadoException : InventarioException
{
    public VideojuegoDuplicadoException(string titulo, string plataforma, string formato)
        : base($"Ya existe '{titulo}' para {plataforma} en formato {formato}.") { }
}

public class BaseDatosException : InventarioException
{
    public BaseDatosException(string mensaje, Exception interna) : base(mensaje, interna) { }
}
