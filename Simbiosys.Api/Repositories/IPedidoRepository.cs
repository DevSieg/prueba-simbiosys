using Simbiosys.Api.Models;

namespace Simbiosys.Api.Repositories;

public interface IPedidoRepository
{
    Task<IEnumerable<PedidoDto>> GetAllAsync();
    Task<IEnumerable<DetallePedidoDto>> GetDetalleAsync(int pedidoId);
    Task<PedidoDto> CrearPedidoAsync(CrearPedidoRequest request);
}
