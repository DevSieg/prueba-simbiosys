using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Simbiosys.Api.Exceptions;
using Simbiosys.Api.Models;

namespace Simbiosys.Api.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly string _connectionString;

    public PedidoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IEnumerable<PedidoDto>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            SELECT Id, CodigoPedido, Cliente, Fecha, Total, Estado
            FROM Pedidos
            ORDER BY Fecha DESC";
        return await connection.QueryAsync<PedidoDto>(sql);
    }

    public async Task<IEnumerable<DetallePedidoDto>> GetDetalleAsync(int pedidoId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"
            SELECT 
                dp.Id,
                dp.ProductoId,
                p.Nombre AS ProductoNombre,
                p.Codigo AS ProductoCodigo,
                dp.Cantidad,
                dp.PrecioUnitario,
                dp.SubTotal
            FROM DetallePedidos dp
            INNER JOIN Productos p ON p.Id = dp.ProductoId
            WHERE dp.PedidoId = @PedidoId";
        return await connection.QueryAsync<DetallePedidoDto>(sql, new { PedidoId = pedidoId });
    }

    public async Task<PedidoDto> CrearPedidoAsync(CrearPedidoRequest request)
    {
        var codigoPedido = Guid.NewGuid().ToString("N")[..8].ToUpper();

        // Build TVP DataTable
        var table = new DataTable();
        table.Columns.Add("ProductoId", typeof(int));
        table.Columns.Add("Cantidad", typeof(int));

        foreach (var item in request.Items)
        {
            table.Rows.Add(item.ProductoId, item.Cantidad);
        }

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "sp_RegistrarPedido";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Cliente", SqlDbType.NVarChar, 200) { Value = request.Cliente });
        command.Parameters.Add(new SqlParameter("@CodigoPedido", SqlDbType.NVarChar, 50) { Value = codigoPedido });

        var tvpParam = command.Parameters.AddWithValue("@Detalle", table);
        tvpParam.SqlDbType = SqlDbType.Structured;
        tvpParam.TypeName = "dbo.DetallePedidoType";

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Message.Contains("Stock insuficiente", StringComparison.OrdinalIgnoreCase))
        {
            throw new StockInsuficienteException(ex.Message, ex);
        }

        // Retrieve the created order
        const string selectSql = @"
            SELECT Id, CodigoPedido, Cliente, Fecha, Total, Estado
            FROM Pedidos
            WHERE CodigoPedido = @CodigoPedido";

        var pedido = await connection.QuerySingleOrDefaultAsync<PedidoDto>(selectSql, new { CodigoPedido = codigoPedido });
        return pedido ?? new PedidoDto { CodigoPedido = codigoPedido, Cliente = request.Cliente };
    }
}
