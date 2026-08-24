namespace Simbiosys.Api.Models;

public class CrearPedidoRequest
{
    public string Cliente { get; set; } = string.Empty;
    public List<ItemPedidoRequest> Items { get; set; } = new();
}

public class ItemPedidoRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
