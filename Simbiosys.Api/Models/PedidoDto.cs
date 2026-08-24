namespace Simbiosys.Api.Models;

public class PedidoDto
{
    public int Id { get; set; }
    public string CodigoPedido { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
}
