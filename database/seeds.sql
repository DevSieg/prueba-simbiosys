USE SimbiosysDB;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Productos)
BEGIN
    INSERT INTO dbo.Productos (Codigo, Nombre, Precio, Stock) VALUES
    ('PRD-001', 'Laptop Lenovo ThinkPad', 3500.00, 15),
    ('PRD-002', 'Mouse Inalámbrico Logitech', 45.90, 120),
    ('PRD-003', 'Teclado Mecánico RGB', 189.90, 60),
    ('PRD-004', 'Monitor 24" Full HD', 690.00, 25),
    ('PRD-005', 'Audífonos Bluetooth', 129.90, 80),
    ('PRD-006', 'Webcam HD 1080p', 99.90, 40),
    ('PRD-007', 'Disco SSD 1TB', 259.00, 50),
    ('PRD-008', 'Memoria RAM 16GB DDR4', 210.00, 70),
    ('PRD-009', 'Cargador USB-C 65W', 65.00, 100),
    ('PRD-010', 'Silla Ergonómica Oficina', 899.00, 10);
END
GO