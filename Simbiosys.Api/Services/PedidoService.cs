using Simbiosys.Api.Models;
using Simbiosys.Api.Repositories;

namespace Simbiosys.Api.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoService(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<IEnumerable<PedidoDto>> GetAllAsync()
    {
        return await _pedidoRepository.GetAllAsync();
    }

    public async Task<IEnumerable<DetallePedidoDto>> GetDetalleAsync(int pedidoId)
    {
        return await _pedidoRepository.GetDetalleAsync(pedidoId);
    }

    public async Task<PedidoDto> CrearPedidoAsync(CrearPedidoRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new ArgumentException("El pedido debe contener al menos un item.");

        foreach (var item in request.Items)
        {
            if (item.Cantidad <= 0)
                throw new ArgumentException($"La cantidad debe ser mayor a 0 para el ProductoId {item.ProductoId}.");
        }

        if (string.IsNullOrWhiteSpace(request.Cliente))
            throw new ArgumentException("El nombre del cliente es requerido.");

        return await _pedidoRepository.CrearPedidoAsync(request);
    }
}
