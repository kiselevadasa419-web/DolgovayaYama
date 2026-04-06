CREATE DATABASE DebtGraphDB;
GO
USE DebtGraphDB;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Debts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DebtorId INT NOT NULL,
    CreditorId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DebtorId) REFERENCES Users(Id),
    FOREIGN KEY (CreditorId) REFERENCES Users(Id)
);
GO

CREATE TABLE ClearedCycles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CycleInfo NVARCHAR(500) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    ClearedAt DATETIME DEFAULT GETDATE()
);
GO

INSERT INTO Users (Name) VALUES ('Джон Сноу'), ('Дейнерис Таргариен'), 
('Тирион Ланнистер'), ('Серсея Ланнистер'), ('Санса Старк');
GO