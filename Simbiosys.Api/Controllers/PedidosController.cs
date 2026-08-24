using Microsoft.AspNetCore.Mvc;
using Simbiosys.Api.Exceptions;
using Simbiosys.Api.Models;
using Simbiosys.Api.Services;

namespace Simbiosys.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pedidos = await _pedidoService.GetAllAsync();
        return Ok(pedidos);
    }

    [HttpGet("{id}/detalle")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var detalle = await _pedidoService.GetDetalleAsync(id);
        return Ok(detalle);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPedidoRequest request)
    {
        try
        {
            var pedido = await _pedidoService.CrearPedidoAsync(request);
            return StatusCode(StatusCodes.Status201Created, pedido);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (StockInsuficienteException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Ocurrió un error inesperado al procesar el pedido." });
        }
    }
}
