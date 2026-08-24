namespace Simbiosys.Api.Exceptions;

public class StockInsuficienteException : Exception
{
    public StockInsuficienteException(string message) : base(message)
    {
    }

    public StockInsuficienteException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
