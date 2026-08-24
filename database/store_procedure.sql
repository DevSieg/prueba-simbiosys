USE SimbiosysDB;
GO

IF OBJECT_ID('dbo.sp_RegistrarPedido', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RegistrarPedido;
GO

CREATE PROCEDURE dbo.sp_RegistrarPedido
    @Cliente      VARCHAR(120),
    @CodigoPedido VARCHAR(50),
    @Detalle      dbo.DetallePedidoType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; -- si hay error de runtime, revierte automáticamente

    -- Validación básica: el detalle no puede venir vacío
    IF NOT EXISTS (SELECT 1 FROM @Detalle)
    BEGIN
        RAISERROR('El pedido debe contener al menos un producto.', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1) Validar stock suficiente para CADA producto solicitado
        DECLARE @ProductoFaltante VARCHAR(100);
        DECLARE @StockDisponible INT;
        DECLARE @CantidadSolicitada INT;

        SELECT TOP 1
            @ProductoFaltante = p.Nombre,
            @StockDisponible = p.Stock,
            @CantidadSolicitada = d.Cantidad
        FROM @Detalle d
        INNER JOIN dbo.Productos p ON p.Id = d.ProductoId
        WHERE p.Stock < d.Cantidad;

        IF @ProductoFaltante IS NOT NULL
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Stock insuficiente para %s (disponible: %d, solicitado: %d).',
                16, 1, @ProductoFaltante, @StockDisponible, @CantidadSolicitada);
            RETURN;
        END

        -- 2) Validar que todos los ProductoId existan
        IF EXISTS (
            SELECT 1 FROM @Detalle d
            LEFT JOIN dbo.Productos p ON p.Id = d.ProductoId
            WHERE p.Id IS NULL
        )
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Uno o más productos del pedido no existen.', 16, 1);
            RETURN;
        END

        -- 3) Calcular el total del pedido
        DECLARE @Total DECIMAL(18,2);
        SELECT @Total = SUM(p.Precio * d.Cantidad)
        FROM @Detalle d
        INNER JOIN dbo.Productos p ON p.Id = d.ProductoId;

        -- 4) Insertar cabecera del pedido
        DECLARE @PedidoId INT;

        INSERT INTO dbo.Pedidos (CodigoPedido, Cliente, Fecha, Total, Estado)
        VALUES (@CodigoPedido, @Cliente, SYSDATETIME(), @Total, 'COMPLETADO');

        SET @PedidoId = SCOPE_IDENTITY();

        -- 5) Insertar detalle del pedido (con precio unitario congelado)
        INSERT INTO dbo.DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario, SubTotal)
        SELECT
            @PedidoId,
            d.ProductoId,
            d.Cantidad,
            p.Precio,
            p.Precio * d.Cantidad
        FROM @Detalle d
        INNER JOIN dbo.Productos p ON p.Id = d.ProductoId;

        -- 6) Descontar stock
        UPDATE p
        SET p.Stock = p.Stock - d.Cantidad
        FROM dbo.Productos p
        INNER JOIN @Detalle d ON d.ProductoId = p.Id;

        COMMIT TRANSACTION;

        -- Devuelve el pedido creado (útil para el backend)
        SELECT
            Id, CodigoPedido, Cliente, Fecha, Total, Estado
        FROM dbo.Pedidos
        WHERE Id = @PedidoId;

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();

        RAISERROR(@ErrMsg, @ErrSeverity, @ErrState);
    END CATCH
END
GO