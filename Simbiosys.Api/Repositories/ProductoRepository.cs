using Dapper;
using Microsoft.Data.SqlClient;
using Simbiosys.Api.Models;

namespace Simbiosys.Api.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly string _connectionString;

    public ProductoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IEnumerable<ProductoDto>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT Id, Codigo, Nombre, Precio, Stock FROM Productos";
        return await connection.QueryAsync<ProductoDto>(sql);
    }
}
