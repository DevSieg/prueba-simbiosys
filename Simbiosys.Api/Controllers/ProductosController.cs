using Microsoft.AspNetCore.Mvc;
using Simbiosys.Api.Repositories;

namespace Simbiosys.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _productoRepository;

    public ProductosController(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productos = await _productoRepository.GetAllAsync();
        return Ok(productos);
    }
}
