using Simbiosys.Api.Models;

namespace Simbiosys.Api.Repositories;

public interface IProductoRepository
{
    Task<IEnumerable<ProductoDto>> GetAllAsync();
}
