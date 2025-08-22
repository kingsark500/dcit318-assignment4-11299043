-- a. Create database
CREATE DATABASE PharmacyDB;
GO

USE PharmacyDB;
GO

-- b. Medicines table
CREATE TABLE Medicines (
    MedicineID INT PRIMARY KEY IDENTITY,
    Name VARCHAR(100),
    Category VARCHAR(100),
    Price DECIMAL(10,2),
    Quantity INT
);
GO

-- c. Sales table
CREATE TABLE Sales (
    SaleID INT PRIMARY KEY IDENTITY,
    MedicineID INT FOREIGN KEY REFERENCES Medicines(MedicineID),
    QuantitySold INT,
    SaleDate DATETIME DEFAULT GETDATE()
);
GO

-- d. Stored Procedures

-- Add Medicine
CREATE PROCEDURE AddMedicine
    @Name VARCHAR(100),
    @Category VARCHAR(100),
    @Price DECIMAL(10,2),
    @Quantity INT
AS
BEGIN
    INSERT INTO Medicines (Name, Category, Price, Quantity)
    VALUES (@Name, @Category, @Price, @Quantity);
END
GO

-- Search Medicine
CREATE PROCEDURE SearchMedicine
    @SearchTerm VARCHAR(100)
AS
BEGIN
    SELECT * FROM Medicines
    WHERE Name LIKE '%' + @SearchTerm + '%'
       OR Category LIKE '%' + @SearchTerm + '%';
END
GO

-- Update Stock
CREATE PROCEDURE UpdateStock
    @MedicineID INT,
    @Quantity INT
AS
BEGIN
    UPDATE Medicines
    SET Quantity = @Quantity
    WHERE MedicineID = @MedicineID;
END
GO

-- Record Sale
CREATE PROCEDURE RecordSale
    @MedicineID INT,
    @QuantitySold INT
AS
BEGIN
    INSERT INTO Sales (MedicineID, QuantitySold, SaleDate)
    VALUES (@MedicineID, @QuantitySold, GETDATE());

    UPDATE Medicines
    SET Quantity = Quantity - @QuantitySold
    WHERE MedicineID = @MedicineID;
END
GO

-- Get All Medicines
CREATE PROCEDURE GetAllMedicines
AS
BEGIN
    SELECT * FROM Medicines;
END
GO