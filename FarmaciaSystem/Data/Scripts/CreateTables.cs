using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Scripts
{
    public static class CreateTablesScript
    {
        public const string SQL = @"
-- Tabla de roles
CREATE TABLE IF NOT EXISTS Roles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedDate TEXT NOT NULL DEFAULT (datetime('now'))
);

-- Tabla de permisos
CREATE TABLE IF NOT EXISTS Permissions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT,
    Module TEXT NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1
);

-- Tabla de roles y permisos
CREATE TABLE IF NOT EXISTS RolePermissions (
    RoleId INTEGER NOT NULL,
    PermissionId INTEGER NOT NULL,
    AssignedDate TEXT NOT NULL DEFAULT (datetime('now')),
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE
);

-- Tabla de almacenes
CREATE TABLE IF NOT EXISTS Warehouses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Location TEXT,
    Manager TEXT,
    Phone TEXT,
    ManagerId INTEGER,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedDate TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (ManagerId) REFERENCES Users(Id)
);

-- Tabla de usuarios
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Email TEXT UNIQUE,
    FullName TEXT NOT NULL,
    Phone TEXT,
    RoleId INTEGER NOT NULL,
    AssignedWarehouseId INTEGER,
    IsActive INTEGER NOT NULL DEFAULT 1,
    LastAccess TEXT,
    CreatedDate TEXT NOT NULL DEFAULT (datetime('now')),
    ModifiedDate TEXT NOT NULL DEFAULT (datetime('now')),
    CreatedBy INTEGER,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (AssignedWarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);

-- Tabla de ubicaciones
CREATE TABLE IF NOT EXISTS Locations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    WarehouseId INTEGER NOT NULL,
    Code TEXT NOT NULL,
    Description TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    UNIQUE (WarehouseId, Code)
);

-- Tabla de proveedores
CREATE TABLE IF NOT EXISTS Suppliers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Phone TEXT,
    Email TEXT
);

-- Tabla de categorías
CREATE TABLE IF NOT EXISTS Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT
);

-- Tabla de principios activos
CREATE TABLE IF NOT EXISTS ActiveIngredients (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT
);

-- Tabla de formas farmacéuticas
CREATE TABLE IF NOT EXISTS PharmaceuticalForms (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT
);

-- Tabla de productos
CREATE TABLE IF NOT EXISTS Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Barcode TEXT UNIQUE,
    Name TEXT NOT NULL,
    ActiveIngredientId INTEGER,
    Concentration TEXT,
    PharmaceuticalFormId INTEGER NOT NULL,
    CategoryId INTEGER,
    SalePrice REAL NOT NULL,
    PurchasePrice REAL NOT NULL,
    RequiresPrescription INTEGER NOT NULL DEFAULT 0,
    MinStock INTEGER NOT NULL DEFAULT 10,
    MaxStock INTEGER NOT NULL DEFAULT 1000,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedDate TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (ActiveIngredientId) REFERENCES ActiveIngredients(Id),
    FOREIGN KEY (PharmaceuticalFormId) REFERENCES PharmaceuticalForms(Id)
);

-- Tabla de lotes
CREATE TABLE IF NOT EXISTS Batches (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    WarehouseId INTEGER NOT NULL,
    LocationId INTEGER NOT NULL,
    BatchNumber TEXT NOT NULL,
    ExpiryDate TEXT NOT NULL,
    CurrentStock INTEGER NOT NULL DEFAULT 0,
    PurchasePrice REAL NOT NULL,
    SupplierId INTEGER,
    EntryDate TEXT NOT NULL DEFAULT (datetime('now')),
    IsActive INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (LocationId) REFERENCES Locations(Id),
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    UNIQUE (ProductId, BatchNumber, LocationId)
);

-- Tabla de movimientos de inventario
CREATE TABLE IF NOT EXISTS InventoryMovements (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    BatchId INTEGER NOT NULL,
    Type INTEGER NOT NULL, -- 0=Entry, 1=Exit, 2=Adjustment, 3=Expired, 4=Return, 5=Transfer
    Quantity INTEGER NOT NULL,
    Reason TEXT,
    User TEXT,
    MovementDate TEXT NOT NULL DEFAULT (datetime('now')),
    Reference TEXT,
    DestinationWarehouseId INTEGER,
    FOREIGN KEY (BatchId) REFERENCES Batches(Id),
    FOREIGN KEY (DestinationWarehouseId) REFERENCES Warehouses(Id)
);

-- Tabla de sesiones de usuario
CREATE TABLE IF NOT EXISTS UserSessions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    SessionToken TEXT NOT NULL UNIQUE,
    StartDate TEXT NOT NULL DEFAULT (datetime('now')),
    ExpiryDate TEXT NOT NULL,
    IpAddress TEXT,
    UserAgent TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Tabla de auditoría
CREATE TABLE IF NOT EXISTS AuditLogs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Action TEXT NOT NULL,
    TableAffected TEXT,
    RecordId INTEGER,
    OldData TEXT,
    NewData TEXT,
    IpAddress TEXT,
    ActionDate TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Tabla de alertas
CREATE TABLE IF NOT EXISTS Alerts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type INTEGER NOT NULL, -- 0=CriticalStock, 1=LowStock, 2=ExpiredProduct, 3=ExpiringProduct
    ProductId INTEGER,
    WarehouseId INTEGER,
    Message TEXT NOT NULL,
    Priority INTEGER NOT NULL, -- 0=Low, 1=Medium, 2=High, 3=Critical
    IsRead INTEGER NOT NULL DEFAULT 0,
    AlertDate TEXT NOT NULL DEFAULT (datetime('now')),
    TargetUser TEXT,
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id)
);

-- Crear índices para mejorar rendimiento
CREATE INDEX IF NOT EXISTS idx_products_barcode ON Products(Barcode);
CREATE INDEX IF NOT EXISTS idx_products_name ON Products(Name);
CREATE INDEX IF NOT EXISTS idx_batches_expiry ON Batches(ExpiryDate);
CREATE INDEX IF NOT EXISTS idx_batches_product ON Batches(ProductId);
CREATE INDEX IF NOT EXISTS idx_inventory_movements_batch ON InventoryMovements(BatchId);
CREATE INDEX IF NOT EXISTS idx_inventory_movements_date ON InventoryMovements(MovementDate);
CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username);
CREATE INDEX IF NOT EXISTS idx_alerts_date ON Alerts(AlertDate);
CREATE INDEX IF NOT EXISTS idx_alerts_read ON Alerts(IsRead);
";
    }
}
