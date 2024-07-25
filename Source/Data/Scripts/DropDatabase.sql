USE master
GO

IF EXISTS (
    SELECT [name]
        FROM sys.databases
        WHERE [name] = N'Comanda'
)
DROP DATABASE Comanda
GO
