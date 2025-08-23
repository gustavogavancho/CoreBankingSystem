-- ============================================================================
-- Core Banking System - Database Schema Script
-- Descripción: Script de creación de base de datos, tablas y datos iniciales
-- Autor: Core Banking System
-- Fecha: $(date)
-- ============================================================================

-- Crear base de datos si no existe
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'CoreBankingSystemDB')
BEGIN
    CREATE DATABASE [CoreBankingSystemDB]
END
GO

USE [CoreBankingSystemDB]
GO

-- ============================================================================
-- ELIMINACIÓN DE TABLAS EXISTENTES (Para desarrollo/testing)
-- ============================================================================
IF OBJECT_ID('dbo.Transactions', 'U') IS NOT NULL DROP TABLE dbo.Transactions;
IF OBJECT_ID('dbo.Accounts', 'U') IS NOT NULL DROP TABLE dbo.Accounts;
IF OBJECT_ID('dbo.Clients', 'U') IS NOT NULL DROP TABLE dbo.Clients;
IF OBJECT_ID('dbo.People', 'U') IS NOT NULL DROP TABLE dbo.People;
GO

-- ============================================================================
-- CREACIÓN DE TABLAS
-- ============================================================================

-- Tabla People (Entidad base para herencia)
CREATE TABLE [dbo].[People] (
    [Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [Name] nvarchar(200) NOT NULL,
    [Gender] nvarchar(50) NULL,
    [Age] int NOT NULL DEFAULT 0,
    [Identification] nvarchar(100) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [Discriminator] nvarchar(max) NOT NULL -- Para EF Core TPH (Table Per Hierarchy)
);
GO

-- Tabla Clients (Hereda de People)
CREATE TABLE [dbo].[Clients] (
    [Id] uniqueidentifier NOT NULL PRIMARY KEY,
    [ClientId] uniqueidentifier NOT NULL UNIQUE,
    [Password] nvarchar(max) NOT NULL,
    [Status] bit NOT NULL DEFAULT 1,
    
    -- Foreign Key a People
    CONSTRAINT [FK_Clients_People_Id] FOREIGN KEY ([Id]) REFERENCES [People] ([Id]) ON DELETE CASCADE
);
GO

-- Índice único para ClientId
CREATE UNIQUE INDEX [IX_Clients_ClientId] ON [Clients] ([ClientId]);
GO

-- Tabla Accounts
CREATE TABLE [dbo].[Accounts] (
    [AccountNumber] nvarchar(30) NOT NULL PRIMARY KEY,
    [AccountType] nvarchar(50) NOT NULL,
    [InitialBalance] decimal(18,2) NOT NULL DEFAULT 0,
    [Status] bit NOT NULL DEFAULT 1,
    [ClientId] uniqueidentifier NOT NULL,
    
    -- Foreign Key a Clients
    CONSTRAINT [FK_Accounts_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [People] ([Id]) ON DELETE CASCADE
);
GO

-- Índice para ClientId en Accounts
CREATE INDEX [IX_Accounts_ClientId] ON [Accounts] ([ClientId]);
GO

-- Tabla Transactions
CREATE TABLE [dbo].[Transactions] (
    [TransactionId] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [Date] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [TransactionType] nvarchar(50) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [AccountNumber] nvarchar(30) NOT NULL,
    
    -- Foreign Key a Accounts
    CONSTRAINT [FK_Transactions_Accounts_AccountNumber] FOREIGN KEY ([AccountNumber]) REFERENCES [Accounts] ([AccountNumber]) ON DELETE CASCADE
);
GO

-- Índice para AccountNumber en Transactions
CREATE INDEX [IX_Transactions_AccountNumber] ON [Transactions] ([AccountNumber]);
-- Índice para Date (para consultas de reporte)
CREATE INDEX [IX_Transactions_Date] ON [Transactions] ([Date]);
GO

-- ============================================================================
-- INSERCIÓN DE DATOS DE PRUEBA
-- ============================================================================

-- Variables para IDs
DECLARE @Client1Id uniqueidentifier = NEWID();
DECLARE @Client2Id uniqueidentifier = NEWID();
DECLARE @Client1BusinessId uniqueidentifier = NEWID();
DECLARE @Client2BusinessId uniqueidentifier = NEWID();

-- Insertar People base (para herencia TPH)
INSERT INTO [People] ([Id], [Name], [Gender], [Age], [Identification], [PhoneNumber], [Discriminator])
VALUES 
    (@Client1Id, 'John Doe', 'Male', 35, 'ID-1001', '+1-555-0100', 'Client'),
    (@Client2Id, 'Jane Smith', 'Female', 31, 'ID-1002', '+1-555-0101', 'Client');
GO

-- Insertar Clients
INSERT INTO [Clients] ([Id], [ClientId], [Password], [Status])
VALUES 
    (@Client1Id, @Client1BusinessId, 'P@ssw0rd1', 1),
    (@Client2Id, @Client2BusinessId, 'P@ssw0rd2', 1);
GO

-- Insertar Accounts
INSERT INTO [Accounts] ([AccountNumber], [AccountType], [InitialBalance], [Status], [ClientId])
VALUES 
    ('ACC-1001', 'Checking', 1000.00, 1, @Client1Id),
    ('ACC-2002', 'Savings', 2500.00, 1, @Client2Id),
    ('ACC-3003', 'Checking', 500.00, 1, @Client1Id);
GO

-- Insertar Transactions con balances calculados
-- Transacciones para ACC-1001 (Balance inicial: 1000)
INSERT INTO [Transactions] ([TransactionId], [Date], [TransactionType], [Amount], [Balance], [AccountNumber])
VALUES 
    (NEWID(), DATEADD(day, -5, GETUTCDATE()), 'Deposit', 500.00, 1500.00, 'ACC-1001'),
    (NEWID(), DATEADD(day, -3, GETUTCDATE()), 'Withdrawal', -200.00, 1300.00, 'ACC-1001');

-- Transacciones para ACC-2002 (Balance inicial: 2500)
INSERT INTO [Transactions] ([TransactionId], [Date], [TransactionType], [Amount], [Balance], [AccountNumber])
VALUES 
    (NEWID(), DATEADD(day, -4, GETUTCDATE()), 'Deposit', 750.00, 3250.00, 'ACC-2002');

-- Transacciones para ACC-3003 (Balance inicial: 500)
INSERT INTO [Transactions] ([TransactionId], [Date], [TransactionType], [Amount], [Balance], [AccountNumber])
VALUES 
    (NEWID(), DATEADD(day, -2, GETUTCDATE()), 'Deposit', 300.00, 800.00, 'ACC-3003');
GO

-- ============================================================================
-- VISTAS ÚTILES PARA CONSULTAS
-- ============================================================================

-- Vista para obtener información completa de clientes
CREATE VIEW [dbo].[vw_ClientsComplete] AS
SELECT 
    p.Id,
    c.ClientId,
    p.Name,
    p.Gender,
    p.Age,
    p.Identification,
    p.PhoneNumber,
    c.Status,
    (SELECT COUNT(*) FROM Accounts a WHERE a.ClientId = p.Id) as AccountCount
FROM People p
INNER JOIN Clients c ON p.Id = c.Id
WHERE p.Discriminator = 'Client';
GO

-- Vista para obtener balances actuales de cuentas
CREATE VIEW [dbo].[vw_AccountBalances] AS
SELECT 
    a.AccountNumber,
    a.AccountType,
    a.InitialBalance,
    a.Status,
    c.Name as ClientName,
    c.ClientId,
    COALESCE(t.CurrentBalance, a.InitialBalance) as CurrentBalance,
    COALESCE(t.LastTransactionDate, NULL) as LastTransactionDate
FROM Accounts a
INNER JOIN People p ON a.ClientId = p.Id
INNER JOIN Clients c ON p.Id = c.Id
LEFT JOIN (
    SELECT 
        t1.AccountNumber,
        t1.Balance as CurrentBalance,
        t1.Date as LastTransactionDate
    FROM Transactions t1
    INNER JOIN (
        SELECT AccountNumber, MAX(Date) as MaxDate
        FROM Transactions
        GROUP BY AccountNumber
    ) t2 ON t1.AccountNumber = t2.AccountNumber AND t1.Date = t2.MaxDate
) t ON a.AccountNumber = t.AccountNumber;
GO

-- Vista para estado de cuenta resumido
CREATE VIEW [dbo].[vw_AccountStatement] AS
SELECT 
    t.AccountNumber,
    a.AccountType,
    p.Name as ClientName,
    c.ClientId,
    t.Date as TransactionDate,
    t.TransactionType,
    t.Amount,
    t.Balance,
    ROW_NUMBER() OVER (PARTITION BY t.AccountNumber ORDER BY t.Date DESC) as TransactionOrder
FROM Transactions t
INNER JOIN Accounts a ON t.AccountNumber = a.AccountNumber
INNER JOIN People p ON a.ClientId = p.Id
INNER JOIN Clients c ON p.Id = c.Id;
GO

-- ============================================================================
-- PROCEDIMIENTOS ALMACENADOS ÚTILES
-- ============================================================================

-- Procedimiento para obtener estado de cuenta por cliente
CREATE PROCEDURE [dbo].[sp_GetClientStatement]
    @ClientIdentification nvarchar(100),
    @StartDate datetime2 = NULL,
    @EndDate datetime2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @StartDate IS NULL SET @StartDate = DATEADD(month, -1, GETUTCDATE());
    IF @EndDate IS NULL SET @EndDate = GETUTCDATE();
    
    SELECT 
        p.Name as ClientName,
        p.Identification,
        a.AccountNumber,
        a.AccountType,
        t.Date as TransactionDate,
        t.TransactionType,
        t.Amount,
        t.Balance
    FROM People p
    INNER JOIN Clients c ON p.Id = c.Id
    INNER JOIN Accounts a ON p.Id = a.ClientId
    INNER JOIN Transactions t ON a.AccountNumber = t.AccountNumber
    WHERE p.Identification = @ClientIdentification
      AND t.Date >= @StartDate
      AND t.Date <= @EndDate
    ORDER BY a.AccountNumber, t.Date DESC;
END
GO

-- ============================================================================
-- CONSTRAINTS Y VALIDACIONES ADICIONALES
-- ============================================================================

-- Constraint para validar que el balance nunca sea negativo en transacciones
ALTER TABLE [Transactions] ADD CONSTRAINT [CK_Transactions_Balance_NonNegative] 
CHECK ([Balance] >= 0);
GO

-- Constraint para validar tipos de transacción
ALTER TABLE [Transactions] ADD CONSTRAINT [CK_Transactions_Type] 
CHECK ([TransactionType] IN ('Deposit', 'Withdrawal', 'Transfer'));
GO

-- Constraint para validar tipos de cuenta
ALTER TABLE [Accounts] ADD CONSTRAINT [CK_Accounts_Type] 
CHECK ([AccountType] IN ('Checking', 'Savings', 'Credit'));
GO

-- ============================================================================
-- ÍNDICES ADICIONALES PARA PERFORMANCE
-- ============================================================================

-- Índice compuesto para consultas de reporte por cliente y fecha
CREATE INDEX [IX_Transactions_ClientDate] ON [Transactions] ([AccountNumber], [Date]) 
INCLUDE ([TransactionType], [Amount], [Balance]);
GO

-- Índice para búsquedas por identificación de cliente
CREATE INDEX [IX_People_Identification] ON [People] ([Identification]) 
WHERE [Identification] IS NOT NULL;
GO

-- ============================================================================
-- FUNCIONES ÚTILES
-- ============================================================================

-- Función para calcular el balance actual de una cuenta
CREATE FUNCTION [dbo].[fn_GetAccountCurrentBalance](@AccountNumber nvarchar(30))
RETURNS decimal(18,2)
AS
BEGIN
    DECLARE @CurrentBalance decimal(18,2);
    
    SELECT @CurrentBalance = t.Balance
    FROM Transactions t
    WHERE t.AccountNumber = @AccountNumber
      AND t.Date = (
          SELECT MAX(Date) 
          FROM Transactions 
          WHERE AccountNumber = @AccountNumber
      );
    
    -- Si no hay transacciones, devolver el balance inicial
    IF @CurrentBalance IS NULL
    BEGIN
        SELECT @CurrentBalance = InitialBalance 
        FROM Accounts 
        WHERE AccountNumber = @AccountNumber;
    END
    
    RETURN ISNULL(@CurrentBalance, 0);
END
GO

-- ============================================================================
-- PERMISOS Y ROLES (Opcional)
-- ============================================================================

-- Crear rol para aplicación
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'CoreBankingApp_Role')
BEGIN
    CREATE ROLE [CoreBankingApp_Role];
END
GO

-- Otorgar permisos al rol
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[People] TO [CoreBankingApp_Role];
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Clients] TO [CoreBankingApp_Role];
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Accounts] TO [CoreBankingApp_Role];
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Transactions] TO [CoreBankingApp_Role];
GRANT SELECT ON [dbo].[vw_ClientsComplete] TO [CoreBankingApp_Role];
GRANT SELECT ON [dbo].[vw_AccountBalances] TO [CoreBankingApp_Role];
GRANT SELECT ON [dbo].[vw_AccountStatement] TO [CoreBankingApp_Role];
GRANT EXECUTE ON [dbo].[sp_GetClientStatement] TO [CoreBankingApp_Role];
GRANT EXECUTE ON [dbo].[fn_GetAccountCurrentBalance] TO [CoreBankingApp_Role];
GO

-- ============================================================================
-- CONSULTAS DE VERIFICACIÓN
-- ============================================================================

-- Verificar datos insertados
SELECT 'Clients' as TableName, COUNT(*) as RecordCount FROM Clients
UNION ALL
SELECT 'Accounts', COUNT(*) FROM Accounts
UNION ALL
SELECT 'Transactions', COUNT(*) FROM Transactions;

-- Mostrar información de clientes y sus cuentas
SELECT * FROM vw_ClientsComplete;

-- Mostrar balances actuales
SELECT * FROM vw_AccountBalances;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================

PRINT 'Base de datos CoreBankingSystem creada exitosamente.';
PRINT 'Tablas creadas: People, Clients, Accounts, Transactions';
PRINT 'Vistas creadas: vw_ClientsComplete, vw_AccountBalances, vw_AccountStatement';
PRINT 'Procedimientos creados: sp_GetClientStatement';
PRINT 'Funciones creadas: fn_GetAccountCurrentBalance';
GO