using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Scripts
{
    public static class InitialDataScript
    {
        public const string SQL = @"
-- Insertar roles básicos
INSERT INTO Roles (Name, Description) VALUES
('Administrador', 'Acceso completo al sistema, gestión de usuarios y configuración'),
('Farmacéutico', 'Gestión de medicamentos, ventas y control de inventario'),
('Auxiliar', 'Operaciones básicas de inventario y ventas'),
('Almacenista', 'Gestión de almacén, entradas y salidas de mercancía'),
('Supervisor', 'Supervisión de operaciones y generación de reportes'),
('Solo Lectura', 'Solo consulta de información, sin modificaciones');

-- Insertar permisos del sistema
INSERT INTO Permissions (Name, Description, Module) VALUES
-- Módulo Productos
('productos_crear', 'Crear nuevos productos', 'productos'),
('productos_editar', 'Editar información de productos', 'productos'),
('productos_eliminar', 'Eliminar productos del sistema', 'productos'),
('productos_consultar', 'Consultar información de productos', 'productos'),
-- Módulo Inventario
('inventario_entrada', 'Registrar entrada de mercancía', 'inventario'),
('inventario_salida', 'Registrar salida de mercancía', 'inventario'),
('inventario_ajuste', 'Realizar ajustes de inventario', 'inventario'),
('inventario_transferencia', 'Transferir productos entre almacenes', 'inventario'),
('inventario_consultar', 'Consultar estado del inventario', 'inventario'),
-- Módulo Usuarios
('usuarios_crear', 'Crear nuevos usuarios', 'usuarios'),
('usuarios_editar', 'Editar información de usuarios', 'usuarios'),
('usuarios_eliminar', 'Eliminar usuarios del sistema', 'usuarios'),
('usuarios_consultar', 'Consultar información de usuarios', 'usuarios'),
-- Módulo Reportes
('reportes_ventas', 'Generar reportes de ventas', 'reportes'),
('reportes_inventario', 'Generar reportes de inventario', 'reportes'),
('reportes_vencimientos', 'Generar reportes de vencimientos', 'reportes'),
('reportes_auditoria', 'Generar reportes de auditoría', 'reportes'),
-- Módulo Configuración
('config_general', 'Configuración general del sistema', 'configuracion'),
('config_almacenes', 'Gestionar almacenes y ubicaciones', 'configuracion'),
('config_proveedores', 'Gestionar proveedores', 'configuracion'),
-- Módulo Alertas
('alertas_gestionar', 'Gestionar alertas del sistema', 'alertas'),
('alertas_consultar', 'Consultar alertas', 'alertas');

-- Asignar permisos a roles
-- Administrador: Todos los permisos
INSERT INTO RolePermissions (RoleId, PermissionId) 
SELECT 1, Id FROM Permissions;

-- Farmacéutico: Productos, inventario, reportes, alertas
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES
(2, 1), (2, 2), (2, 4), -- productos
(2, 5), (2, 6), (2, 7), (2, 9), -- inventario
(2, 14), (2, 15), (2, 16), -- reportes
(2, 20), (2, 21); -- alertas

-- Auxiliar: Operaciones básicas
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES
(3, 4), -- productos consultar
(3, 5), (3, 6), (3, 9), -- inventario básico
(3, 21); -- alertas consultar

-- Almacenista: Gestión de almacén
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES
(4, 4), -- productos consultar
(4, 5), (4, 6), (4, 7), (4, 8), (4, 9), -- inventario completo
(4, 15), -- reportes inventario
(4, 19), -- config almacenes
(4, 20), (4, 21); -- alertas

-- Supervisor: Reportes y supervisión
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES
(5, 4), (5, 9), -- consultas
(5, 14), (5, 15), (5, 16), (5, 17), -- todos los reportes
(5, 13), -- usuarios consultar
(5, 21); -- alertas consultar

-- Solo Lectura: Solo consultas
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES
(6, 4), (6, 9), (6, 13), (6, 21);

-- Insertar categorías básicas
INSERT INTO Categories (Name, Description) VALUES
('Analgésicos', 'Medicamentos para el dolor'),
('Antibióticos', 'Medicamentos contra infecciones'),
('Vitaminas', 'Suplementos vitamínicos'),
('Digestivos', 'Medicamentos para problemas digestivos'),
('Dermatológicos', 'Productos para la piel');

-- Insertar proveedores básicos
INSERT INTO Suppliers (Name, Phone, Email) VALUES
('Distribuidora Médica SA', '555-0101', 'ventas@distmedica.com'),
('Farma Supply Ltda', '555-0102', 'pedidos@farmasupply.com'),
('Laboratorios Unidos', '555-0103', 'contacto@labunidos.com');

-- Insertar principios activos básicos
INSERT INTO ActiveIngredients (Name, Description) VALUES
('Paracetamol', 'Analgésico y antipirético'),
('Amoxicilina', 'Antibiótico de amplio espectro'),
('Ácido Ascórbico', 'Vitamina C'),
('Omeprazol', 'Inhibidor de la bomba de protones'),
('Ibuprofeno', 'Antiinflamatorio no esteroideo');

-- Insertar formas farmacéuticas básicas
INSERT INTO PharmaceuticalForms (Name, Description) VALUES
('Tableta', 'Forma sólida comprimida'),
('Cápsula', 'Forma sólida encapsulada'),
('Jarabe', 'Forma líquida dulce'),
('Crema', 'Forma semisólida para uso tópico'),
('Inyección', 'Forma líquida para administración parenteral'),
('Gotas', 'Forma líquida en pequeñas dosis');

-- Insertar almacenes básicos
INSERT INTO Warehouses (Name, Location, Manager, Phone) VALUES
('Almacén Principal', 'Planta Baja - Sector A', 'Juan Pérez', '555-1001'),
('Almacén Refrigerado', 'Planta Baja - Sector B', 'María García', '555-1002'),
('Farmacia Mostrador', 'Planta Alta - Venta', 'Ana López', '555-1003'),
('Almacén Controlados', 'Sótano - Sector Seguro', 'Carlos Ruiz', '555-1004');

-- Insertar ubicaciones básicas
INSERT INTO Locations (WarehouseId, Code, Description) VALUES
-- Almacén Principal (ID: 1)
(1, 'A1', 'Estante A1'),
(1, 'A2', 'Estante A2'),
(1, 'B1', 'Estante B1'),
(1, 'B2', 'Estante B2'),
(1, 'C1', 'Gaveta C1'),
-- Almacén Refrigerado (ID: 2)
(2, 'REF1', 'Refrigerador 1'),
(2, 'REF2', 'Refrigerador 2'),
(2, 'CONG', 'Congelador'),
-- Farmacia Mostrador (ID: 3)
(3, 'VIT1', 'Vitrina 1'),
(3, 'VIT2', 'Vitrina 2'),
(3, 'MOST', 'Estante Mostrador'),
(3, 'CAJ1', 'Cajón 1'),
-- Almacén Controlados (ID: 4)
(4, 'SEG1', 'Caja Seguridad 1'),
(4, 'SEG2', 'Caja Seguridad 2'),
(4, 'CTRL', 'Estante Controlados');

-- Insertar usuario administrador por defecto
INSERT INTO Users (Username, PasswordHash, Email, FullName, Phone, RoleId, IsActive) VALUES
('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'admin@farmacia.com', 'Administrador Sistema', '555-0000', 1, 1);

-- Insertar productos de ejemplo
INSERT INTO Products (Barcode, Name, ActiveIngredientId, Concentration, PharmaceuticalFormId, CategoryId, SalePrice, PurchasePrice, RequiresPrescription, MinStock, MaxStock) VALUES
('7501234567890', 'Paracetamol 500mg', 1, '500mg', 1, 1, 5.50, 3.20, 0, 50, 500),
('7501234567891', 'Amoxicilina 250mg', 2, '250mg', 2, 2, 12.80, 8.50, 1, 30, 300),
('7501234567892', 'Vitamina C 1000mg', 3, '1000mg', 1, 3, 8.90, 5.60, 0, 25, 250),
('7501234567893', 'Omeprazol 20mg', 4, '20mg', 2, 4, 15.20, 9.80, 0, 40, 400),
('7501234567894', 'Ibuprofeno 400mg', 5, '400mg', 1, 1, 7.80, 4.90, 0, 35, 350);
";
    }
}

