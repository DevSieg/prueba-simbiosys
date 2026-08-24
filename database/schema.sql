IF DB_ID('SimbiosysDB') IS NULL
BEGIN
    CREATE DATABASE SimbiosysDB;
END
GO

USE SimbiosysDB;
GO

-- =========================================================
-- Tabla: Productos
-- =========================================================
IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL DROP TABLE dbo.Productos;
GO

CREATE TABLE dbo.Productos (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Codigo   VARCHAR(20)   NOT NULL,
    Nombre   VARCHAR(100)  NOT NULL,
    Precio   DECIMAL(18,2) NOT NULL CHECK (Precio >= 0),
    Stock    INT           NOT NULL CHECK (Stock >= 0),
    CONSTRAINT UQ_Productos_Codigo UNIQUE (Codigo)
);
GO

-- =========================================================
-- Tabla: Pedidos
-- =========================================================
IF OBJECT_ID('dbo.Pedidos', 'U') IS NOT NULL DROP TABLE dbo.Pedidos;
GO

CREATE TABLE dbo.Pedidos (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    CodigoPedido  VARCHAR(50)   NOT NULL,
    Cliente       VARCHAR(120)  NOT NULL,
    Fecha         DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    Total         DECIMAL(18,2) NOT NULL DEFAULT 0,
    Estado        VARCHAR(20)   NOT NULL DEFAULT 'COMPLETADO',
    CONSTRAINT UQ_Pedidos_CodigoPedido UNIQUE (CodigoPedido),
    CONSTRAINT CK_Pedidos_Estado CHECK (Estado IN ('COMPLETADO', 'CANCELADO', 'PENDIENTE'))
);
GO

-- =========================================================
-- Tabla: DetallePedidos
-- =========================================================
IF OBJECT_ID('dbo.DetallePedidos', 'U') IS NOT NULL DROP TABLE dbo.DetallePedidos;
GO

CREATE TABLE dbo.DetallePedidos (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId       INT NOT NULL,
    ProductoId     INT NOT NULL,
    Cantidad       INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    SubTotal       DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_DetallePedidos_Pedidos
        FOREIGN KEY (PedidoId) REFERENCES dbo.Pedidos(Id) ON DELETE CASCADE,
    CONSTRAINT FK_DetallePedidos_Productos
        FOREIGN KEY (ProductoId) REFERENCES dbo.Productos(Id)
);
GO

-- Índices de apoyo para consultas frecuentes
CREATE INDEX IX_DetallePedidos_PedidoId ON dbo.DetallePedidos(PedidoId);
CREATE INDEX IX_DetallePedidos_ProductoId ON dbo.DetallePedidos(ProductoId);
GO

-- =========================================================
-- Tipo de tabla (TVP) para enviar el detalle del pedido
-- al Stored Procedure sp_RegistrarPedido
-- =========================================================
IF TYPE_ID('dbo.DetallePedidoType') IS NOT NULL
    DROP TYPE dbo.DetallePedidoType;
GO

CREATE TYPE dbo.DetallePedidoType AS TABLE (
    ProductoId INT NOT NULL,
    Cantidad   INT NOT NULL
);
GO