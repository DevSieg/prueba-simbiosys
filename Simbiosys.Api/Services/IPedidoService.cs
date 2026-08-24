using Simbiosys.Api.Models;

namespace Simbiosys.Api.Services;

public interface IPedidoService
{
    Task<IEnumerable<PedidoDto>> GetAllAsync();
    Task<IEnumerable<DetallePedidoDto>> GetDetalleAsync(int pedidoId);
    Task<PedidoDto> CrearPedidoAsync(CrearPedidoRequest request);
}
